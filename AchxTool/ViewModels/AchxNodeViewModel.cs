using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels;

public abstract partial class AchxNodeViewModel : ObservableObject
{
    [ObservableProperty]
    private string? _name;

    [ObservableProperty]
    private bool _isSelected;
}