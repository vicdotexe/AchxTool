using System.Linq.Expressions;

using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using Avalonia.Threading;

using Microsoft.Extensions.DependencyInjection;
using Avalonia;
using AchxTool.Views;
using AchxTool.ViewModels;
using System.Reflection;
using System.Runtime.CompilerServices;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Microsoft.Extensions.DependencyInjection.Extensions;
using AchxTool.Services;

namespace AchxTool;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddAvaloniaServices();
        services.AddSingleton<IMessenger, WeakReferenceMessenger>();
        services.Scan<ObservableObject>(typeof(ServiceCollectionExtensions).Assembly,
            static (isp, t) => isp.AddTransient(t));
        services.AddLooseFactories(typeof(ServiceCollectionExtensions).Assembly);
        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        
        services.AddAchx();
    }
    private static void AddAchx(this IServiceCollection services)
    {
        services.AddViews();
        services.AddSingleton<CanvasViewModel>();
        services.AddSingleton<IBitmapBank, BitmapBank>();
    }


    private static void AddAvaloniaServices(this IServiceCollection services)
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

        services.AddSingleton(sp => sp.GetRequiredService<TopLevel>().StorageProvider);
    }

    private static void AddViews(this IServiceCollection services)
    {
        //NB: Window is only needed for Desktop
        services.AddTransient<MainWindow>();

        services.AddView<MainView, MainViewModel>();
    }

    private static void AddView<TView, TViewModel>(this IServiceCollection services)
        where TView : class
        where TViewModel : class
    {
        services.AddTransient<TViewModel>();
        services.AddTransient<TView>();
    }

    private static IServiceCollection Scan<TBaseType>(this IServiceCollection services, Assembly assembly,
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
}
