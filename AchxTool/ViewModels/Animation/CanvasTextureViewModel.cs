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
    [NotifyPropertyChangedFor(nameof(Image))]
    private string? _imageSource;

    public Bitmap? Image => ImageSource is not null ? BitmapBank.Get(ImageSource) : null;

    public bool IsDragEnabled { get; set; } = false;
    public bool IsResizeEnabled { get; set; } = false;

    public bool IsSelectionEnabled { get; set; } = false;

    private IBitmapBank BitmapBank { get; }

    public CanvasTextureViewModel(IBitmapBank bitmapBank)
    {
        BitmapBank = bitmapBank;

        PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(Image))
            {
                Width = Image?.Size.Width ?? 0;
                Height = Image?.Size.Height ?? 0;
            }
        };
    }
}