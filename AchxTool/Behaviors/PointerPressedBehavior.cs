using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using Avalonia;
using System.Windows.Input;

namespace AchxTool.Behaviors;
public class PointerPressedBehavior : Behavior<Control>
{
    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<PointerPressedBehavior, ICommand?>(nameof(Command));

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<PointerPressedBehavior, object?>(nameof(CommandParameter));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        if (AssociatedObject != null)
        {
            AssociatedObject.PointerPressed += OnPointerPressed;
        }
    }

    protected override void OnDetaching()
    {
        if (AssociatedObject != null)
        {
            AssociatedObject.PointerPressed -= OnPointerPressed;
        }

        base.OnDetaching();
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (Command?.CanExecute(CommandParameter) == true)
        {
            Command.Execute(CommandParameter);
        }
    }
}