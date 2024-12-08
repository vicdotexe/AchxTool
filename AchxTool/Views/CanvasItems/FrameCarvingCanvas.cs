using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.LogicalTree;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Media;
using Avalonia.Rendering.Composition;
using Avalonia.VisualTree;

namespace AchxTool.Views.CanvasItems
{
    public class FrameCarvingCanvas : SelectingItemsControl
    {
        static FrameCarvingCanvas()
        {
            AffectsMeasure<FrameCarvingCanvas>();
        }

        protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
        {
            return new DraggableCanvasItem();
        }

        protected override bool NeedsContainerOverride(object? item, int index, out object? recycleKey)
        {
            recycleKey = null;
            return item is not DraggableCanvasItem;
        }
    }

    



    public class ResizeAdorner : Grid
    {
        //private Control AdornedElement { get; }
        //private double _downX;
        //private double _downY;

        //public ResizeAdorner(Control adornedElement)
        //{
        //    AdornedElement = adornedElement;

        //}

        //protected override void OnPointerExited(PointerEventArgs e)
        //{
        //    base.OnPointerExited(e);
        //    IsVisible = false;
        //}

        //private void Thumb_DragStarted(object? sender, VectorEventArgs e)
        //{
        //    _downX = AdornedElement.GetValue(Canvas.LeftProperty);
        //    _downY = AdornedElement.GetValue(Canvas.TopProperty);
        //}

        //private void Thumb_DragDelta(object? sender, VectorEventArgs e)
        //{
        //    AdornedElement.SetCurrentValue(Canvas.LeftProperty, _downX + e.Vector.X);
        //    AdornedElement.SetCurrentValue(Canvas.TopProperty, _downY + e.Vector.Y);
        //}
    }
}