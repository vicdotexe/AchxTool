namespace AchxTool.ViewModels.Animation;

public interface ICanvasItem
{
    double X { get; set; }
    double Y { get; set; }
    double Width { get; set; }
    double Height { get; set; }
    double Z { get; set; }
    bool IsSelectionEnabled { get; set; }
    bool IsDragEnabled { get; set; }
    bool IsResizeEnabled { get; set; }
}

public interface IHaveTexture
{
    TextureViewModel? Texture { get; }
}