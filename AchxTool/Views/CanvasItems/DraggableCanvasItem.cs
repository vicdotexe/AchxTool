using Avalonia.Controls.Metadata;
using Avalonia.Controls.Presenters;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.VisualTree;
using Avalonia.Controls.PanAndZoom;

namespace AchxTool.Views.CanvasItems;

[PseudoClasses(":isSelected")]
public class DraggableCanvasItem : ContentControl
{
    public static readonly StyledProperty<double> SnapToGridProperty =
        AvaloniaProperty.Register<DraggableCanvasItem, double>(nameof(SnapToGrid));

    private Point _pressedOffset;
    private bool _isDragging;

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


    public FrameCarvingCanvas? FrameCarver { get; private set; }

    private Canvas? Canvas { get; set; }

    public static readonly DirectProperty<DraggableCanvasItem, bool> IsSelectedProperty =
        AvaloniaProperty.RegisterDirect<DraggableCanvasItem, bool>(nameof(IsSelected), o => o.IsSelected);


    public bool IsSelected
    {
        get => FrameCarver?.SelectedItem == Content;
    }

    public DraggableCanvasItem()
    {
        AttachedToVisualTree += OnAttachedToVisualTree;
    }

    private void OnAttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        Canvas = this.FindAncestorOfType<Canvas>();

        if (FrameCarver is not null)
        {
            FrameCarver.PropertyChanged -= WatchFrameCarver;
        }

        FrameCarver = this.FindAncestorOfType<FrameCarvingCanvas>();

        if (FrameCarver is not null)
        {
            FrameCarver.PropertyChanged += WatchFrameCarver;
        }



        void WatchFrameCarver(object? sender, AvaloniaPropertyChangedEventArgs e)
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
        
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && FrameCarver is not null)
        {
            FrameCarver.SelectedItem = Content;
            _isDragging = IsDragEnabled;
            _pressedOffset = e.GetPosition(this);
            e.Handled = true;
        }
        base.OnPointerPressed(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (_isDragging && Canvas is not null)
        {
            var (x, y) = e.GetPosition(Canvas);
            SetCurrentValue(Canvas.LeftProperty, Math.Round(x - _pressedOffset.X));
            SetCurrentValue(Canvas.TopProperty, Math.Round(y - _pressedOffset.Y));
        }
        base.OnPointerMoved(e);
    }

    protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
    {
        base.OnPointerCaptureLost(e);
        _isDragging = false;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        FrameCarver = this.FindAncestorOfType<FrameCarvingCanvas>();
    }
}