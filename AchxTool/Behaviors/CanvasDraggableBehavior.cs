using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Xaml.Interactivity;
using Avalonia;
using System;

using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;

namespace AchxTool.Behaviors
{
    public class CanvasDraggableBehavior : Behavior<Control>
    {
        private Point _elementDownPosition;

        private bool _isDragging;
        private Canvas? _parentCanvas;
        private ContentPresenter? _contentPresenter;
        private Control? Target => _contentPresenter ?? AssociatedObject;

        // Attachable properties to allow customization
        public static readonly AttachedProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<CanvasDraggableBehavior, Control, bool>("IsEnabled", true);

        public static bool GetIsEnabled(Control element) => element.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(Control element, bool value) => element.SetValue(IsEnabledProperty, value);

        public static readonly StyledProperty<double> SnapToGridProperty =
            AvaloniaProperty.Register<CanvasDraggableBehavior, double>(nameof(SnapToGrid));

        public double SnapToGrid
        {
            get => GetValue(SnapToGridProperty);
            set => SetValue(SnapToGridProperty, value);
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.PointerPressed += OnPointerPressed;
                AssociatedObject.PointerMoved += OnPointerMoved;
                AssociatedObject.PointerReleased += OnPointerReleased;
            }
        }

        protected override void OnDetaching()
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.PointerPressed -= OnPointerPressed;
                AssociatedObject.PointerMoved -= OnPointerMoved;
                AssociatedObject.PointerReleased -= OnPointerReleased;
            }

            base.OnDetaching();
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            if (AssociatedObject is null || !GetIsEnabled(AssociatedObject) || AssociatedObject.FindAncestorOfType<Canvas>() is not { } canvas)
                return;

            _parentCanvas = canvas;
            //_contentPresenter = AssociatedObject.FindAncestorOfType<ContentPresenter>();

            if (e.GetCurrentPoint(AssociatedObject) is { Properties.IsLeftButtonPressed: true } pointer)
            {
                _isDragging = true;
                _elementDownPosition = pointer.Position;
                e.Pointer.Capture(AssociatedObject);
            }


        }

        private void OnPointerMoved(object? sender, PointerEventArgs e)
        {
            if (!_isDragging || Target is null || _parentCanvas is null)
            {
                return;
            }

            Point current = e.GetCurrentPoint(_parentCanvas).Position;
            Point destination = current - _elementDownPosition;

            if (SnapToGrid is var multiple and > 0)
            {
                destination = new Point(SnapToNearest(destination.X, multiple), SnapToNearest(destination.Y, multiple));
            }

            Target.SetCurrentValue(Canvas.LeftProperty, destination.X);
            Target.SetCurrentValue(Canvas.TopProperty, destination.Y);

            static double SnapToNearest(double value, double multiple)
            {
                return multiple * Math.Round(value / multiple);
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

    public class CanvasDraggable
    {
        public static readonly AttachedProperty<bool> IsDraggableProperty =
            AvaloniaProperty.RegisterAttached<CanvasDraggable, Control, bool>("IsDraggable");

        static CanvasDraggable()
        {
            IsDraggableProperty.Changed.Subscribe(OnIsDraggableChanged);
        }

        public static bool GetIsDraggable(Control control) => control.GetValue(IsDraggableProperty);
        public static void SetIsDraggable(Control control, bool value) => control.SetValue(IsDraggableProperty, value);

        private static void OnIsDraggableChanged(AvaloniaPropertyChangedEventArgs<bool> args)
        {
            if (args.Sender is not Control target)
                return;

            var behaviors = Interaction.GetBehaviors(target);

            if (args.NewValue.Value)
            {
                if (!behaviors.OfType<CanvasDraggableBehavior>().Any())
                {
                    behaviors.Add(new CanvasDraggableBehavior());
                }
            }
            else
            {
                var draggableBehavior = behaviors.OfType<CanvasDraggableBehavior>().FirstOrDefault();
                if (draggableBehavior != null)
                {
                    behaviors.Remove(draggableBehavior);
                }
            }
        }
    }
}