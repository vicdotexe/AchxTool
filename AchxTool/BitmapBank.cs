using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace AchxTool
{
    public interface IBitmapBank
    {
        Bitmap Get(string filePath);
    }

    public class BitmapBank : IBitmapBank
    {
        private Dictionary<string, Bitmap> Bitmaps { get; } = [];

        public Bitmap Get(string filePath)
        {
            if (Bitmaps.TryGetValue(filePath, out var bitmap))
            {
                return bitmap;
            }

            try
            {
                bitmap = new Bitmap(filePath);
                Bitmaps[filePath] = bitmap;
                return bitmap;
            }
            catch (Exception e)
            {
                try
                {
                    bitmap = new Bitmap(AssetLoader.Open(new Uri($"avares://AchxTool/Assets/{filePath}")));
                    Bitmaps[filePath] = bitmap;
                    return bitmap;
                }
                catch (Exception e2)
                {
                    Console.WriteLine($"Failed to load bitmap from {filePath}: {e2.Message}");
                    return null;
                }

                Console.WriteLine($"Failed to load bitmap from {filePath}: {e.Message}");
                return null;
            }
        }
    }
}