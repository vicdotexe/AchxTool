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

    public static readonly StyledProperty<bool> IsSelectionEnabledProperty =
        AvaloniaProperty.Register<DraggableCanvasItem, bool>(nameof(IsSelectionEnabled), true);

    public bool IsSelectionEnabled
    {
        get => GetValue(IsSelectionEnabledProperty);
        set => SetValue(IsSelectionEnabledProperty, value);
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
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed && FrameCarver is not null)
        {
            if (IsSelectionEnabled)
            {
                FrameCarver.SelectedItem = Content;
            }
                
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
                RaisePropertyChanged(IsSelectedProperty, e.OldValue == Content, e.NewValue == Content);
                PseudoClasses.Set(":isSelected", e.NewValue == Content);
            }
        }

        base.OnAttachedToVisualTree(e);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        List<Thumb?> thumbs =
        [
            e.NameScope.Find<Thumb>("TopLeftHandle"),
            e.NameScope.Find<Thumb>("TopHandle"),
            e.NameScope.Find<Thumb>("TopRightHandle"),
            e.NameScope.Find<Thumb>("RightHandle"),
            e.NameScope.Find<Thumb>("BottomRightHandle"),
            e.NameScope.Find<Thumb>("BottomHandle"),
            e.NameScope.Find<Thumb>("BottomLeftHandle"),
            e.NameScope.Find<Thumb>("LeftHandle")
        ];

        foreach (Thumb? thumb in thumbs)
        {
            if (thumb is null)
            {
                continue;
            }
            thumb.DragDelta += (sender, e) =>
            {
                if (Canvas is not null && sender is Thumb {Name: var name})
                {
                    double left = Canvas.GetLeft(this);
                    double top = Canvas.GetTop(this);
                    double right = left + this.Width;
                    double bottom = top + this.Height;

                    switch (name)
                    {
                        case "TopLeftHandle":
                            left += e.Vector.X;
                            top += e.Vector.Y;
                            break;
                        case "TopHandle":
                            top += e.Vector.Y;
                            break;
                        case "TopRightHandle":
                            right += e.Vector.X;
                            top += e.Vector.Y;
                            break;
                        case "RightHandle":
                            right += e.Vector.X;
                            break;
                        case "BottomRightHandle":
                            right += e.Vector.X;
                            bottom += e.Vector.Y;
                            break;
                        case "BottomHandle":
                            bottom += e.Vector.Y;
                            break;
                        case "BottomLeftHandle":
                            left += e.Vector.X;
                            bottom += e.Vector.Y;
                            break;
                        case "LeftHandle":
                            left += e.Vector.X;
                            break;
                    }

                    if (SnapToGrid > 0)
                    {
                        left = Math.Round(left / SnapToGrid) * SnapToGrid;
                        top = Math.Round(top / SnapToGrid) * SnapToGrid;
                        right = Math.Round(right / SnapToGrid) * SnapToGrid;
                        bottom = Math.Round(bottom / SnapToGrid) * SnapToGrid;
                    }

                    this.SetCurrentValue(WidthProperty, Math.Max(1, right - left));
                    this.SetCurrentValue(HeightProperty, Math.Max(1, bottom - top));
                    this.SetCurrentValue(Canvas.LeftProperty, left);
                    this.SetCurrentValue(Canvas.TopProperty, top);
                }
            };
        }
    }
}