using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace AchxTool.Views.CanvasItems;

[PseudoClasses(":isSelected")]
public class DraggableCanvasItem : ContentPresenter
{
    public static readonly StyledProperty<double> SnapToGridProperty =
        AvaloniaProperty.Register<DraggableCanvasItem, double>(nameof(SnapToGrid));

    private TranslationAdorner TransformAdorner { get; }

    public double SnapToGrid
    {
        get => GetValue(SnapToGridProperty);
        set => SetValue(SnapToGridProperty, value);
    }

    public static readonly StyledProperty<bool> IsDragEnabledProperty =
        AvaloniaProperty.Register<DraggableCanvasItem, bool>(nameof(IsDragEnabled), true);

    public bool IsDragEnabled
    {
        get => GetValue(IsDragEnabledProperty);
        set => SetValue(IsDragEnabledProperty, value);
    }


    public FrameCarvingCanvas? Canvas { get; private set; }

    public static readonly DirectProperty<DraggableCanvasItem, bool> IsSelectedProperty =
        AvaloniaProperty.RegisterDirect<DraggableCanvasItem, bool>(nameof(IsSelected), o => o.IsSelected);


    public bool IsSelected
    {
        get => Canvas?.SelectedItem == Content;
    }

    public DraggableCanvasItem()
    {
        TransformAdorner = new TranslationAdorner(this);
        AttachedToVisualTree += (s, e) =>
        {
            if (AdornerLayer.GetAdornerLayer(this) is { } layer)
            {

                TransformAdorner.IsVisible = false;
                layer.Children.Add(TransformAdorner);
                AdornerLayer.SetAdornedElement(TransformAdorner, this);
            }
        };
        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        if (Canvas is not null)
        {
            Canvas.PropertyChanged -= WatchCanvas;
        }

        Canvas = this.FindAncestorOfType<FrameCarvingCanvas>();

        if (Canvas is not null)
        {
            Canvas.PropertyChanged += WatchCanvas;
        }

        void WatchCanvas(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property.Name == nameof(FrameCarvingCanvas.SelectedItem))
            {
                RaisePropertyChanged(IsSelectedProperty, e.OldValue is true, e.NewValue is true);
                PseudoClasses.Set(":isSelected", ReferenceEquals(e.NewValue, Content));
            }
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && Canvas is not null)
        {
            Canvas.SelectedItem = Content;
        }
    }

    protected override void OnPointerEntered(PointerEventArgs e)
    {
        base.OnPointerEntered(e);
        TransformAdorner.IsVisible = true;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        Canvas = this.FindAncestorOfType<FrameCarvingCanvas>();
    }
}