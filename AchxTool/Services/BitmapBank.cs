using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

using AchxTool.ViewModels;

using Avalonia.Media.Imaging;
using Avalonia.Platform;

using CommunityToolkit.Mvvm.Messaging;

using Mono.Collections.Generic;

namespace AchxTool.Services;

public interface ITextureProvider
{
    Bitmap? Get(FileInfo? fileInfo);
}

public class TextureProvider : ITextureProvider, IRecipient<ProjectLoadedMessage>
{
    private Dictionary<FileInfo, Bitmap> Textures { get; } = new(new FileInfoComparer());

    public TextureProvider(IMessenger messenger)
    {
        messenger.RegisterAll(this);
    }

    public Bitmap? Get(FileInfo? fileInfo)
    {
        if (fileInfo is null)
        {
            return null;
        }

        if (Textures.TryGetValue(fileInfo, out var bitmap))
        {
            return bitmap;
        }

        try
        {
            bitmap = new Bitmap(fileInfo.FullName);
            Textures[fileInfo] = bitmap;
            return bitmap;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to load bitmap from {fileInfo.FullName}: {e.Message}");

            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string relativeAssetPath = Path.GetRelativePath(currentDirectory, fileInfo.FullName);

                bitmap = new Bitmap(AssetLoader.Open(new Uri($"avares://AchxTool/Assets/{relativeAssetPath}")));
                Textures[fileInfo] = bitmap;
                return bitmap;
            }
            catch (Exception e2)
            {
                Console.WriteLine($"Could not find avalonia asset {fileInfo}: {e2.Message}");
            }
        }

        return null;
    }

    void IRecipient<ProjectLoadedMessage>.Receive(ProjectLoadedMessage message)
    {
        Textures.Clear();

        foreach (var animation in message.Project.Animations)
        {
            foreach (var frame in animation.Frames)
            {
                if (frame.TextureFile is { } fileInfo)
                {
                    _ = Get(fileInfo);
                }

            }
        }
    }

    public class FileInfoComparer : IEqualityComparer<FileInfo>
    {
        public bool Equals(FileInfo? x, FileInfo? y)
        {
            // Handle nulls
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            // Compare full paths
            return string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(FileInfo? obj)
        {
            // Use FullName for hash code generation (case-insensitive)
            return obj?.FullName?.ToLowerInvariant().GetHashCode() ?? 0;
        }
    }

}

public static class FileInfoExtensions
{
    public static bool Matches(this FileInfo? self, FileInfo? other)
    {
        if (self is null || other is null)
        {
            return false;
        }

        return string.Equals(self.FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
    }
}