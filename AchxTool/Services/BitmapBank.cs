using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

using AchxTool.ViewModels;
using AchxTool.ViewModels.Nodes;

using Avalonia.Media.Imaging;
using Avalonia.Platform;

using CommunityToolkit.Mvvm.Messaging;

using Mono.Collections.Generic;

namespace AchxTool.Services;

public interface ITextureManager : IEnumerable<TextureViewModel>
{
    TextureViewModel Load(FileInfo fileInfo);
}

public class TextureManager : ITextureManager, IList, INotifyCollectionChanged, IRecipient<ProjectLoadedMessage>
{
    private static readonly FileInfoComparer _fileInfoComparer = new();

    private Dictionary<FileInfo, TextureViewModel> Textures { get; } = new(_fileInfoComparer);

    public TextureManager(IMessenger messenger)
    {
        messenger.RegisterAll(this);
    }

    public TextureViewModel Load(FileInfo fileInfo)
    {
        if (Textures.TryGetValue(fileInfo, out TextureViewModel? texture))
        {
            return texture;
        }

        // todo: default this to a broken image resource and remove nullability on TextureViewModel.Bitmap
        Bitmap? bitmap = null;
        try
        {
            bitmap = new Bitmap(fileInfo.FullName);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to load bitmap from {fileInfo.FullName}: {e.Message}");

            try
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string relativeAssetPath = Path.GetRelativePath(currentDirectory, fileInfo.FullName);

                bitmap = new Bitmap(AssetLoader.Open(new Uri($"avares://AchxTool/Assets/{relativeAssetPath}")));
            }
            catch (Exception e2)
            {
                Console.WriteLine($"Could not find avalonia asset {fileInfo}: {e2.Message}");
            }
        }

        texture = Textures[fileInfo] = new(fileInfo, bitmap);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, texture,
            (this as IList).IndexOf(texture)));
        return texture;
    }

    void IRecipient<ProjectLoadedMessage>.Receive(ProjectLoadedMessage message)
    {
        Textures.Clear();

        HashSet<FileInfo?> allSources = message.Project.Animations.Aggregate(
            new HashSet<FileInfo?>(_fileInfoComparer),
            (infos, animation) =>
            {
                infos.Add(animation.TextureFile);
                foreach (FrameViewModel frame in animation.Frames)
                {
                    infos.Add(frame.TextureFile);
                }

                return infos;
            });

        foreach (FileInfo fileInfo in allSources.OfType<FileInfo>())
        {
            _ = Load(fileInfo);
        }
    }

    public IEnumerator<TextureViewModel> GetEnumerator()
    {
        return Textures.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public class FileInfoComparer : IEqualityComparer<FileInfo?>
    {
        public bool Equals(FileInfo? x, FileInfo? y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;

            return string.Equals(x.FullName, y.FullName, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(FileInfo? obj)
        {
            return obj?.FullName?.ToLowerInvariant().GetHashCode() ?? 0;
        }
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;


#region IList Implementation
// This is a workaround because Avalonia's ItemSourceView expects INotifyPropertyChanged to be paired with an IList implementation.
    int IList.Add(object? value)
    {
        TextureViewModel result = value switch
        {
            FileInfo fileInfo => Load(fileInfo),
            TextureViewModel textureVm when !Textures.ContainsKey(textureVm.FileInfo) => Load(textureVm.FileInfo),
            _ => throw new InvalidOperationException()
        };

        return (this as IList).IndexOf(result.FileInfo);
    }

    void IList.Clear()
    {
        Textures.Clear();
    }

    bool IList.Contains(object? value)
    {
        return value switch
        {
            FileInfo fileInfo => Textures.ContainsKey(fileInfo),
            TextureViewModel texture => Textures.ContainsValue(texture),
            _ => throw new NotImplementedException()
        };
    }

    int IList.IndexOf(object? value)
    {
        return value switch
        {
            FileInfo fileInfo => Textures.Keys.ToList().IndexOf(fileInfo),
            TextureViewModel texture => Textures.Values.ToList().IndexOf(texture),
            _ => throw new NotImplementedException()
        };
    }

    void IList.Insert(int index, object? value)
    {
        throw new NotImplementedException();
    }

    void IList.Remove(object? value)
    {
        switch (value)
        {
            case FileInfo fileInfo:
                Textures.Remove(fileInfo);
                break;
            case TextureViewModel texture:
                Textures.Remove(texture.FileInfo);
                break;
            default:
                throw new NotImplementedException();
        }
    }

    void IList.RemoveAt(int index)
    {
        FileInfo fileInfo = Textures.Keys.ElementAt(index);
        Textures.Remove(fileInfo);
    }

    bool IList.IsFixedSize { get; } = false;

    bool IList.IsReadOnly { get; } = false;

    object? IList.this[int index]
    {
        get
        {
            FileInfo fileInfo = Textures.Keys.ElementAt(index);
            return Textures[fileInfo];
        }
        set => throw new NotImplementedException();
    }

    void ICollection.CopyTo(Array array, int index)
    {
        throw new NotImplementedException();
    }

    public int Count => Textures.Count;

    bool ICollection.IsSynchronized { get; } = false;

    object ICollection.SyncRoot { get; } = new();
#endregion
}

public static class TextureManagerExtensions
{
    public static TextureViewModel? Get(this ITextureManager self, FileInfo? fileInfo) =>
        fileInfo is null ? null : self.Load(fileInfo);
}