using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AchxTool.Views
{
    public class FrameCarvingCanvas : ItemsControl
    {
        //depdendcy property for selected item
        public static readonly StyledProperty<object?> SelectedItemProperty =
            AvaloniaProperty.Register<FrameCarvingCanvas, object?>(nameof(SelectedItem), defaultBindingMode: BindingMode.TwoWay);

        public object? SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set => SetValue(SelectedItemProperty, value);
        }

        static FrameCarvingCanvas()
        {
            // Associate the default style with this control
            AffectsMeasure<FrameCarvingCanvas>();
            SelectedItemProperty.Changed.AddClassHandler<FrameCarvingCanvas>((s, e) => s.OnSelectedItemChanged(e));
        }

        private void OnSelectedItemChanged(AvaloniaPropertyChangedEventArgs avaloniaPropertyChangedEventArgs)
        {
            //foreach (var item in this.ItemsPanelRoot.Children.OfType<>())
        }

        public FrameCarvingCanvas()
        {

        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new DraggableCanvasItem();
        }

    }

    public class DraggableCanvasItem : ContentPresenter
    {
        public static readonly StyledProperty<double> SnapToGridProperty =
            AvaloniaProperty.Register<DraggableCanvasItem, double>(nameof(SnapToGrid));

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

        private Point _offsetDown;
        private bool _isDragging;
        private Canvas? _canvas;

        public DraggableCanvasItem()
        {
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);
            _canvas = e.Parent as Canvas;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (!IsDragEnabled)
            {
                return;
            }

            if (e.GetCurrentPoint(this) is { Properties.IsLeftButtonPressed: true } pointer && _canvas is not null)
            {
                _isDragging = true;
                _offsetDown = pointer.Position;
                e.Pointer.Capture(this);
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging && _canvas is not null)
            {
                (double x, double y) = e.GetPosition(_canvas);

                double xNew = x - _offsetDown.X;
                double yNew = y - _offsetDown.Y;

                if (SnapToGrid > 0)
                {
                    xNew = Math.Round(xNew / SnapToGrid) * SnapToGrid;
                    yNew = Math.Round(yNew / SnapToGrid) * SnapToGrid;
                }

                SetCurrentValue(Canvas.LeftProperty, xNew);
                SetCurrentValue(Canvas.TopProperty, yNew);
            }
        }

        private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        {
            if (_isDragging)
            {
                _isDragging = false;
                e.Pointer.Capture(null);
            }
        }
    }
}