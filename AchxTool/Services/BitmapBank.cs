using AchxTool.ViewModels;

using Avalonia.Media.Imaging;
using Avalonia.Platform;

using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.Services;

public interface IBitmapBank
{
    Bitmap Get(string filePath);
}

public class BitmapBank : IBitmapBank, IRecipient<ProjectLoadedMessage>
{
    private Dictionary<string, Bitmap> Bitmaps { get; } = [];
    private DirectoryInfo? ParentDirectory { get; set; }

    public BitmapBank(IMessenger messenger)
    {
        messenger.RegisterAll(this);
    }

    public Bitmap Get(string filePath)
    {
        if (Bitmaps.TryGetValue(filePath, out var bitmap))
        {
            return bitmap;
        }

        try
        {
            string actualPath = Path.Combine(ParentDirectory?.FullName ?? "", filePath);
            bitmap = new Bitmap(actualPath);
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

    void IRecipient<ProjectLoadedMessage>.Receive(ProjectLoadedMessage message)
    {
        ParentDirectory = new FileInfo(message.Project.FilePath!).Directory;

        foreach (var animation in message.Project.Animations)
        {
            foreach (var frame in animation.Frames)
            {
                if (frame.TextureName is { } path)
                {
                    _ = Get(path);
                }

            }
        }
    }
}