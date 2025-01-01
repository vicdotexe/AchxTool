using AchxTool.Services;

using Avalonia.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels.Animation;
public partial class CanvasTextureViewModel : ObservableObject, ICanvasItem
{
    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private double _z;

    [ObservableProperty]
    private double _width;

    [ObservableProperty]
    private double _height;

    [ObservableProperty] 
    private Bitmap? _bitmap;

    public bool IsDragEnabled { get; set; } = false;
    public bool IsResizeEnabled { get; set; } = false;

    public bool IsSelectionEnabled { get; set; } = false;

    partial void OnBitmapChanged(Bitmap? value)
    {
        Width = value?.Size.Width ?? 0;
        Height = value?.Size.Height ?? 0;
    }
}