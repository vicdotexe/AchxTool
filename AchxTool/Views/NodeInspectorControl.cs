using Avalonia;
using Avalonia.Controls;

namespace AchxTool.Views
{
    public class NodeInspectorControl : ContentControl
    {
        public static readonly StyledProperty<object?> SelectedNodeProperty =
            AvaloniaProperty.Register<NodeInspectorControl, object?>(nameof(SelectedNode));

        public object? SelectedNode
        {
            get => GetValue(SelectedNodeProperty);
            set => SetValue(SelectedNodeProperty, value);
        }

        // Constructor to link SelectedObject to Content
        public NodeInspectorControl()
        {
            this.Bind(ContentProperty, this.GetObservable(SelectedNodeProperty));
        }
    }
}