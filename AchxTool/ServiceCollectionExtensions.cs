using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

using AchxTool.Services;
using AchxTool.ViewModels;
using AchxTool.ViewModels.Animation;
using AchxTool.Views;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AchxTool;

public static class ServiceCollectionExtensions
{
    public static void AddAchx(this IServiceCollection services)
    {
        services.AddAvaloniaServices()
            .AddMvvmHelpers()
            .AddAchxServices();
    }

    private static IServiceCollection AddAchxServices(this IServiceCollection services)
    {
        // views
        services.AddTransient<MainWindow>();
        services.AddSingleton<MainView>();

        // singleton viewmodels
        services.AddSingleton<MainViewModel>();
        services.AddSingleton<CanvasViewModel>();
        services.AddSingleton<NodeTreeViewModel>();

        // services
        services.AddSingleton<IBitmapBank, BitmapBank>();
        services.AddSingleton<IProjectLoader, ProjectLoader>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<INodeTree>(sp => sp.GetRequiredService<NodeTreeViewModel>());

        return services;
    }

    private static IServiceCollection AddMvvmHelpers(this IServiceCollection services)
    {
        services.AddSingleton<IMessenger, WeakReferenceMessenger>();
        services.Scan<ObservableObject>(typeof(ServiceCollectionExtensions).Assembly,
            static (isp, t) => isp.AddTransient(t));
        services.AddLooseFactories(typeof(ServiceCollectionExtensions).Assembly);
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();

        return services;
    }


    private static IServiceCollection AddAvaloniaServices(this IServiceCollection services)
    {
        services.AddSingleton<IDispatcher>(_ => Dispatcher.UIThread);
        services.AddSingleton(_ => Application.Current?.ApplicationLifetime ?? throw new InvalidOperationException("No application lifetime is set"));

        services.AddSingleton(sp =>
            sp.GetRequiredService<IApplicationLifetime>() switch
            {
                IClassicDesktopStyleApplicationLifetime desktop => desktop.MainWindow ?? throw new InvalidOperationException("No main window set"),
                ISingleViewApplicationLifetime singleViewPlatform => TopLevel.GetTopLevel(singleViewPlatform.MainView) ?? throw new InvalidOperationException("Could not find top level element for single view"),
                _ => throw new InvalidOperationException($"Could not find {nameof(TopLevel)} element"),
            }
        );

        services.AddSingleton(sp => new Lazy<IStorageProvider>(() => sp.GetRequiredService<TopLevel>().StorageProvider));
        return services;
    }
}

file static class Helpers
{
    public static IServiceCollection Scan<TBaseType>(this IServiceCollection services, Assembly assembly,
    Action<IServiceCollection, Type> callback)
    {
        const BindingFlags ctorFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        Type baseType = typeof(TBaseType);

        IEnumerable<Type> closedTypes = assembly
            .DefinedTypes.Select(t => t.AsType())
            .Where(t => t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition
                        && baseType.IsAssignableFrom(t) && !t.IsDelegate() && !t.IsCompilerGenerated()
                        && t.GetConstructors(ctorFlags).Length > 0);

        foreach (Type type in closedTypes)
        {
            callback(services, type);
        }

        return services;
    }

    public static IServiceCollection AddLooseFactories(this IServiceCollection services, Assembly assembly)
    {
        const BindingFlags ctorFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

        HashSet<Type> functions = new(assembly
            .DefinedTypes.Select(t => t.AsType())
            .SelectMany(t => t.GetConstructors())
            .SelectMany(c => c.GetParameters().Select(pi => pi.ParameterType))
            .Where(t => t.IsGenericType && t.IsDelegate() && t.Name.StartsWith($"{nameof(Func<int>)}`"))
            .Where(funcType =>
            {
                Type t = funcType.GenericTypeArguments[^1];
                return t.IsClass && !t.IsAbstract && !t.IsGenericTypeDefinition
                       && !t.IsDelegate() && !t.IsCompilerGenerated()
                       && t.GetConstructors(ctorFlags).Length > 0;
            }));

        foreach (Type funcType in functions)
        {
            services.TryAddSingleton(funcType, isp =>
            {
                return isp.CompileFunc(funcType);
            });
        }

        return services;
    }

    public static bool IsDelegate(this Type type)
    {
        return type.IsSubclassOf(typeof(Delegate));
    }

    public static bool IsCompilerGenerated(this Type type)
    {
        return type.GetCustomAttributes<CompilerGeneratedAttribute>().Any();
    }

    public static Delegate CompileFunc(this IServiceProvider provider, Type funcType)
    {
        // This is roughly equivalent to ActivatorUtilities.GetServiceOrCreateInstance,
        //  but we extend it to allow for Func parameters.
        Type[] arguments = funcType.GenericTypeArguments;
        Type returnType = arguments[^1];

        Expression serviceProvider = Expression.Constant(provider);
        Expression typeExpression = Expression.Constant(returnType);

        ParameterExpression[] parameters = arguments[..^1]
            .Select(static (a, i) => Expression.Parameter(a, name: $"arg{i}"))
            .ToArray();

        Expression create = Expression.Call(CreateInstance, serviceProvider, typeExpression,
            Expression.NewArrayInit(typeof(object), parameters.Select(x => Expression.Convert(x, typeof(object)))));
        Expression get = Expression.Call(serviceProvider, GetService, typeExpression);

        Expression call = (returnType, parameters) switch
        {
            // CreateInstance will fail for these, so only include GetService
            ({ IsAbstract: true }
                or { IsInterface: true }, _) => get,

            // If there are no parameters, we include the GetService call to try resolving
            //  from the container on the chance there's a service registered for the type.
            (_, { Length: 0 }) => Expression.Coalesce(get, create),

            _ => create,
        };

        Expression cast = Expression.Convert(call, returnType);
        LambdaExpression returnFunc = Expression.Lambda(funcType, cast, parameters);
        return returnFunc.Compile();
    }

    private static MethodInfo GetService { get; } = typeof(IServiceProvider).GetMethod(nameof(IServiceProvider.GetService))
                                                    ?? throw new InvalidOperationException($"Could not find method {nameof(IServiceProvider.GetService)}");
    private static MethodInfo CreateInstance { get; } = typeof(ActivatorUtilities).GetMethod(nameof(ActivatorUtilities.CreateInstance),
                                                            new[] { typeof(IServiceProvider), typeof(Type), typeof(object[]) })
                                                        ?? throw new InvalidOperationException($"Could not find method {nameof(ActivatorUtilities.CreateInstance)}");

    private static IServiceCollection AddLazy<TService>(this IServiceCollection services)
        where TService : notnull
    {
        services.AddSingleton(sp => new Lazy<TService>(() => sp.GetRequiredService<TService>()));
        return services;
    }
}