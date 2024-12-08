using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace AchxTool.Views.CanvasItems;

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