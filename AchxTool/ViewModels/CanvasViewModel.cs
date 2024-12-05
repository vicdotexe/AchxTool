using System.Collections.ObjectModel;

using Avalonia.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels
{
    public class CanvasViewModel : ObservableObject
    {
        public ObservableCollection<ICanvasItem> Items { get; } = [];

        public CanvasTextureViewModel TextureViewModel { get; }

        public CanvasViewModel()
        {
            TextureViewModel = new() { Z = -1 };
            Items.Add(TextureViewModel);
        }
    }

    public interface ICanvasItem
    {
        double X { get; set; }
        double Y { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        double Z { get; set; }
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
        private double _width;

        [ObservableProperty]
        private double _height;

        [ObservableProperty]
        private Bitmap? _image;

        public CanvasTextureViewModel()
        {
            Image = new Bitmap("""C:\Users\vicdo\Downloads\image.png""");
        }
    }
}