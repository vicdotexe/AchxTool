using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AchxTool.Views
{
    public class FrameBoxView : Border
    {
        public static readonly StyledProperty<bool> IsSelectedProperty = AvaloniaProperty.Register<FrameBoxView, bool>(nameof(IsSelected));

        public static readonly StyledProperty<double> XProperty = AvaloniaProperty.Register<FrameBoxView, double>(nameof(X), defaultBindingMode:BindingMode.TwoWay);
        public double X
        {
            get => GetValue(XProperty);
            set
            {
                SetValue(XProperty, value);
                //if (Parent is ContentPresenter parent)
                //{
                //    parent.SetCurrentValue(Canvas.LeftProperty, value);
                //}
            }
        }

        private void SetCurrentX(double value)
        {
            this.SetCurrentValue(XProperty, value);
            if (Parent is ContentPresenter parent)
            {
                parent.SetCurrentValue(Canvas.LeftProperty, value);
            }
        }

        private void Changed(AvaloniaPropertyChangedEventArgs e)
        {

        }

        public static readonly StyledProperty<double> YProperty = AvaloniaProperty.Register<FrameBoxView, double>(nameof(Y), defaultBindingMode: BindingMode.TwoWay);
        public double Y
        {
            get => GetValue(YProperty);
            set
            {
                SetValue(YProperty, value);
                //if (Parent is ContentPresenter parent)
                //{
                //    parent.SetCurrentValue(Canvas.TopProperty, value);
                //}
            }
        }

        private void SetCurrentY(double value)
        {
            this.SetCurrentValue(YProperty, value);
            if (Parent is ContentPresenter parent)
            {
                parent.SetCurrentValue(Canvas.TopProperty, value);
            }
        }

        private void SetCurrentPosition(Point value)
        {
            SetCurrentX(value.X);
            SetCurrentY(value.Y);
        }

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
            XProperty.Changed.Subscribe(Changed);
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

            if (this.FindAncestorOfType<Canvas>() is { } canvas)
            {
                _canvasDown = e.GetPosition(canvas);
                _originalLocation = e.GetPosition(this);
            }
            base.OnPointerPressed(e);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            if (_isDragging)
            {
                if (this.FindAncestorOfType<Canvas>() is { } canvas)
                {
                    SetCurrentPosition(e.GetPosition(canvas) - _originalLocation);
                }
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