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
    private FileInfo? _imageSource;

    public Bitmap? Image => TextureProvider.Get(ImageSource);

    public bool IsDragEnabled { get; set; } = false;
    public bool IsResizeEnabled { get; set; } = false;

    public bool IsSelectionEnabled { get; set; } = false;

    private ITextureProvider TextureProvider { get; }

    public CanvasTextureViewModel(ITextureProvider textureProvider)
    {
        TextureProvider = textureProvider;

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