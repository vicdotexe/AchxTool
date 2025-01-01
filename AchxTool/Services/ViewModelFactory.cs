using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.Extensions.DependencyInjection;

namespace AchxTool.Services;

public interface IViewModelFactory
{
    T New<T>(Action<T>? setup = null) where T : ObservableObject;

    Func<T> NewFactory<T>(Action<T>? setup = null) where T : ObservableObject;
}

public class ViewModelFactory : IViewModelFactory
{
    private IServiceProvider ServiceProvider { get; }
    public ViewModelFactory(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public T New<T>(Action<T>? setup = null) where T : ObservableObject
    {
        T obj = ServiceProvider.GetRequiredService<T>();
        setup?.Invoke(obj);
        return obj;
    }

    public Func<T> NewFactory<T>(Action<T>? setup = null) where T : ObservableObject
    {
        return () => New(setup);
    }
}