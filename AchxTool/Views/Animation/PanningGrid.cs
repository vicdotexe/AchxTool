using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AchxTool.Views.Animation;

public class PanningGrid : ContentControl
{
    public static readonly StyledProperty<ITransform> OffsetProperty =
        AvaloniaProperty.Register<PanningGrid, ITransform>(nameof(Offset), defaultValue: new TranslateTransform());

    public ITransform Offset
    {
        get => GetValue(OffsetProperty);
        set => SetValue(OffsetProperty, value);
    }

    static PanningGrid()
    {
        AffectsRender<PanningGrid>(OffsetProperty);
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        DrawGridLines(context);
    }

    private void DrawGridLines(DrawingContext context)
    {
        var bounds = Bounds; // The visible area of the control
        var spacing = 50;    // Grid spacing in logical world units

        // Get the total transform (scale + pan + center)

        // Inverse of the full RenderTransform to map screen to world
        var transformGroup = Offset.Value;
        var inverseTransform = Offset.Value.Invert();

        // Map the top-left and bottom-right corners of the viewport to world space
        var worldTopLeft = inverseTransform.Transform(new Point(0, 0));
        var worldBottomRight = inverseTransform.Transform(new Point(bounds.Width, bounds.Height));

        // Calculate the world-space bounds of the gridlines to render
        double startX = Math.Floor(worldTopLeft.X / spacing) * spacing;
        double startY = Math.Floor(worldTopLeft.Y / spacing) * spacing;
        double endX = Math.Ceiling(worldBottomRight.X / spacing) * spacing;
        double endY = Math.Ceiling(worldBottomRight.Y / spacing) * spacing;

        var pen = new Pen(Brushes.Gray, 1);
        var axisPen = new Pen(Brushes.White, 1);

        // Draw vertical gridlines
        for (double x = startX; x <= endX; x += spacing)
        {
            // Map world X to screen space
            var screenStart = transformGroup.Transform(new Point(x, worldTopLeft.Y));
            var screenEnd = transformGroup.Transform(new Point(x, worldBottomRight.Y));

            context.DrawLine(x == 0 ? axisPen : pen, screenStart, screenEnd);
        }

        // Draw horizontal gridlines
        for (double y = startY; y <= endY; y += spacing)
        {
            // Map world Y to screen space
            var screenStart = transformGroup.Transform(new Point(worldTopLeft.X, y));
            var screenEnd = transformGroup.Transform(new Point(worldBottomRight.X, y));

            context.DrawLine(y == 0 ? axisPen : pen, screenStart, screenEnd);
        }
    }
}