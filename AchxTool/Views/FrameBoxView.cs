using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AchxTool.Views
{
    public class FrameBoxView : Border
    {
        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<FrameBoxView, bool>(nameof(IsSelected));
        public bool IsSelected
        {
            get => GetValue(IsSelectedProperty);
            set => SetValue(IsSelectedProperty, value);
        }

        bool _isDragging;
        Point _canvasDown;
        private Point _originalLocation;

        public FrameBoxView()
        {
            
        }

        protected override void OnPointerEntered(PointerEventArgs e)
        {
            base.OnPointerEntered(e);
        }

        protected override void OnPointerExited(PointerEventArgs e)
        {
            _isDragging = false;
            base.OnPointerExited(e);
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            _isDragging = true;
            var test = Parent;
            if (this.FindAncestorOfType<Canvas>() is { } canvas && DataContext is CanvasItemViewModel vm)
            {
                _canvasDown = e.GetPosition(canvas);
                _originalLocation = new(vm.X, vm.Y);
            }
            base.OnPointerPressed(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (_isDragging && this.FindAncestorOfType<Canvas>() is { } canvas && DataContext is CanvasItemViewModel vm)
            {
                var pos = e.GetPosition(canvas);
                var delta = pos - _canvasDown;
                vm.X = _originalLocation.X + delta.X;
                vm.Y = _originalLocation.Y + delta.Y;
            }
            base.OnPointerMoved(e);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            _isDragging = false;
            base.OnPointerReleased(e);
        }
        protected override void OnPointerCaptureLost(PointerCaptureLostEventArgs e)
        {
            _isDragging = false;
            base.OnPointerCaptureLost(e);
        }
    }
}