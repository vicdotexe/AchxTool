using Avalonia;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Controls.Primitives;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;

namespace AchxTool.Views.CanvasItems;

public class TranslationAdorner : Thumb
{
    private Control AdornedElement { get; }
    private double _downX;
    private double _downY;
    private ZoomBorder? _zoomBorder;

    public TranslationAdorner(Control adornedElement)
    {
        AdornedElement = adornedElement;
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        IsVisible = false;
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            base.OnPointerPressed(e);
            e.Handled = false;
            AdornedElement.RaiseEvent(e);
        }
        else
        {
            e.Source = AdornedElement;
            AdornedElement.RaiseEvent(e);
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            base.OnPointerReleased(e);
        }
        else
        {
            e.Source = AdornedElement;
            AdornedElement.RaiseEvent(e);
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            base.OnPointerMoved(e);
        }
        else
        {
            e.Source = AdornedElement;
            AdornedElement.RaiseEvent(e);
        }
    }

    protected override void OnDragStarted(VectorEventArgs e)
    {
        _downX = AdornedElement.GetValue(Canvas.LeftProperty);
        _downY = AdornedElement.GetValue(Canvas.TopProperty);
        _zoomBorder = AdornedElement.FindAncestorOfType<ZoomBorder>();
        e.Handled = true;
    }

    protected override void OnDragDelta(VectorEventArgs e)
    {
        Vector delta = e.Vector;
        if (_zoomBorder is not null)
        {
            delta = new Vector(delta.X / _zoomBorder.ZoomX, delta.Y / _zoomBorder.ZoomY);
        }
        AdornedElement.SetCurrentValue(Canvas.LeftProperty, Math.Round(_downX + delta.X));
        AdornedElement.SetCurrentValue(Canvas.TopProperty, Math.Round(_downY + delta.Y));
        base.OnDragDelta(e);
    }

    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        AdornedElement.RaiseEvent(e);
    }
}