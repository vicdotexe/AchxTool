using System.Collections.ObjectModel;

using AchxTool.Services;

using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels;

public partial class TextureManagerViewModel : ObservableObject
{

    public ITextureManager TextureManager { get; }
    private IDialogService DialogService { get; }

    public TextureManagerViewModel(ITextureManager textureManager, IDialogService dialogService)
    {
        TextureManager = textureManager;
        DialogService = dialogService;
    }

    [RelayCommand]
    public async Task PromptNewTextures()
    {
        IReadOnlyList<FileInfo> result = await DialogService.ShowFilePickerAsync(new()
        {
            AllowMultiple = true, FileTypeFilter = [FilePickerFileTypes.ImageAll], Title = "Add Texture"
        });

        foreach (FileInfo fileInfo in result)
        {
            TextureManager.Load(fileInfo);
        }
    }


}

public partial class TextureViewModel : ObservableObject
{
    public FileInfo FileInfo { get; }
    public Bitmap? Bitmap { get; }
    public string Name => FileInfo.Name;

    public TextureViewModel(FileInfo fileInfo, Bitmap? bitmap)
    {
        FileInfo = fileInfo;
        Bitmap = bitmap;
    }
}