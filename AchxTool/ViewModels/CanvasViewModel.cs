using System.Collections.ObjectModel;

using Avalonia.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels;
public partial class CanvasViewModel : ObservableObject
{
    public ObservableCollection<ICanvasItem> Items { get; } = [];

    public CanvasTextureViewModel TextureViewModel { get; }

    [ObservableProperty]
    private ICanvasItem? _selectedItem;

    public CanvasViewModel(Func<CanvasTextureViewModel> textureVmFactory)
    {
        var textureVm = textureVmFactory();
        textureVm.ImageSource = "test-spritesheet.png";
        textureVm.Z = -1;
        TextureViewModel = textureVm;

        Items.Add(TextureViewModel);
    }

    partial void OnSelectedItemChanged(ICanvasItem? value)
    {
        TextureViewModel.ImageSource = value is AnimationFrameViewModel frame ? frame.TextureName : null;
    }
}

public interface ICanvasItem
{
    double X { get; set; }
    double Y { get; set; }
    double? Width { get; set; }
    double? Height { get; set; }
    double Z { get; set; }
    bool IsDragEnabled { get; set; }
    bool IsResizeEnabled { get; set; }
}

public partial class CanvasTextureViewModel : ObservableObject, ICanvasItem
{
    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private double _z;

    [ObservableProperty]
    private double? _width;

    [ObservableProperty]
    private double? _height;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Image))]
    private string? _imageSource;

    public Bitmap? Image => ImageSource is not null ? BitmapBank.Get(ImageSource) : null;

    public bool IsDragEnabled { get; set; } = false;
    public bool IsResizeEnabled { get; set; } = false;

    private IBitmapBank BitmapBank { get; }

    public CanvasTextureViewModel(IBitmapBank bitmapBank)
    {
        BitmapBank = bitmapBank;
    }
}