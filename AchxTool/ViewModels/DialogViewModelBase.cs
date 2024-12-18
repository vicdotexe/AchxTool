using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AchxTool.ViewModels;

public abstract partial class DialogViewModelBase : ObservableObject
{
    public event Action<bool>? Close;

    [ObservableProperty] public string? _title;

    [ObservableProperty] public string? _negativeText;
    [ObservableProperty] public string? _affirmativeText = "OK";

    [RelayCommand]
    public void Affirmative()
    {
        Close?.Invoke(true);
    }

    [RelayCommand]
    public void Negative()
    {
        Close?.Invoke(false);
    }
}