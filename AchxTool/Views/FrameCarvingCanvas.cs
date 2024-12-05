using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Input;

namespace AchxTool.Views
{
    public class FrameCarvingCanvas : ItemsControl
    {
        static FrameCarvingCanvas()
        {
            // Associate the default style with this control
            AffectsMeasure<FrameCarvingCanvas>();

        }

        public FrameCarvingCanvas()
        {

        }

        protected override void PrepareContainerForItemOverride(Control container, object? item, int index)
        { 
            base.PrepareContainerForItemOverride(container, item, index);
        }
    }

    public class DraggableCanvasItem : ContentPresenter
    {
        private Point _lastPointerPosition;
        private bool _isDragging;

        public DraggableCanvasItem()
        {
            PointerPressed += OnPointerPressed;
            PointerMoved += OnPointerMoved;
            PointerReleased += OnPointerReleased;
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var pointer = e.GetCurrentPoint(this);

            if (pointer.Properties.IsLeftButtonPressed)
            {
                _isDragging = true;
                _lastPointerPosition = pointer.Position;
                e.Pointer.Capture(this);
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (_isDragging)
            {
                var pointer = e.GetCurrentPoint(this);

                var delta = pointer.Position - _lastPointerPosition;

                var left = Canvas.GetLeft(this) + delta.X;
                var top = Canvas.GetTop(this) + delta.Y;

                Canvas.SetLeft(this, left);
                Canvas.SetTop(this, top);

                _lastPointerPosition = pointer.Position;
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