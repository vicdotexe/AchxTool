using AchxTool.ViewModels;

using Avalonia.Controls;
using Avalonia.Input;

namespace AchxTool.Views;

public partial class AnimationPreview : UserControl
{
    public AnimationPreview()
    {
        InitializeComponent();
    }

    private void InputElement_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is AnimationRunnerViewModel {IsRunning: true} vm)
        {
            vm.IsRunning = false;
        }
    }

    private void InputElement_OnGotFocus(object? sender, GotFocusEventArgs e)
    {
        if (DataContext is AnimationRunnerViewModel { IsRunning: true } vm)
        {
            vm.IsRunning = false;
        }
    }
}