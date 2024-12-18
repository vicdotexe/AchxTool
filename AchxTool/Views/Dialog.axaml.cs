using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;

namespace AchxTool.Views;

public partial class Dialog : ContentControl
{
    public static readonly AttachedProperty<string?> TitleProperty =
        AvaloniaProperty.RegisterAttached<Dialog, Control, string?>("DialogTitle");
    public static string? GetTitle(Control element) => element.GetValue(TitleProperty);
    public static void SetTitle(Control element, string? value) => element.SetValue(TitleProperty, value);

    public static readonly AttachedProperty<object> ActionsProperty =
        AvaloniaProperty.RegisterAttached<Dialog, Control, object>("DialogActions");
    public static object GetActions(Control element) => element.GetValue(ActionsProperty);
    public static void SetActions(Control element, object value) => element.SetValue(ActionsProperty, value);

    private static DialogDataTemplate DialogDataTemplate { get; set; } = new();

    public Dialog()
    {
        InitializeComponent();
        DialogContent.ContentTemplate = DialogDataTemplate;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        if (DialogContent.Presenter?.Child is { } dialogView)
        {
            Bind(TitleProperty, AvaloniaObjectExtensions.GetObservable(dialogView, TitleProperty));
            Bind(ActionsProperty, AvaloniaObjectExtensions.GetObservable(dialogView, ActionsProperty));
        }
    }
}

public class DialogDataTemplate : IDataTemplate
{
    private readonly Dictionary<Type, Func<Control>> _map;

    public DialogDataTemplate()
    {
        List<Type> viewModelTypes =
        [
            .. typeof(DialogDataTemplate).Assembly.GetTypes()
                .Where(t => typeof(DialogViewModelBase).IsAssignableFrom(t) && !t.IsAbstract)
        ];

        Dictionary<string, Type> viewTypes = typeof(DialogDataTemplate).Assembly.GetTypes()
            .Where(t => typeof(Control).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(t => t.Name, t => t);

        _map = viewModelTypes
            .Select(viewModelType =>
            {
                string viewName = viewModelType.Name.Replace("ViewModel", "View");
                if (viewTypes.TryGetValue(viewName, out Type? viewType))
                {
                    return (viewModelType, viewFactory: (Func<Control>)(() => (Control)Activator.CreateInstance(viewType)));
                }
                throw new InvalidOperationException($"No matching view found for {viewModelType.Name}");
            })
            .ToDictionary(match => match.viewModelType, match => match.viewFactory);
    }

    public Control? Build(object? param)
    {
        return param is null ? null : _map[param.GetType()]();
    }

    public bool Match(object? data)
    {
        return data is DialogViewModelBase;
    }
}