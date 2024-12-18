using AchxTool.ViewModels;
using AchxTool.Views;

using Avalonia.Controls;
using Avalonia.Platform.Storage;

using DialogHostAvalonia;

namespace AchxTool.Services;
public interface IDialogService
{
    Task<bool> ShowAsync<TDialogViewModel>(TDialogViewModel? viewModel = null)
        where TDialogViewModel : DialogViewModelBase;
    Task<IReadOnlyList<FileInfo>> ShowFilePickerAsync(FilePickerOpenOptions? options = null);
}
public class DialogService : IDialogService
{
    private IViewModelFactory ViewModelFactory { get; }
    private Lazy<IStorageProvider> StorageProvider { get; }

    public DialogService(IViewModelFactory viewModelFactory, Lazy<IStorageProvider> storageProvider)
    {
        ViewModelFactory = viewModelFactory;
        StorageProvider = storageProvider;
    }

    public async Task<bool> ShowAsync<TDialogViewModel>(TDialogViewModel? viewModel = null)
        where TDialogViewModel : DialogViewModelBase
    {
        viewModel ??= ViewModelFactory.New<TDialogViewModel>();
        viewModel.Close += x => DialogHost.Close(null, x);

        Control view = new Dialog();
        view.DataContext = viewModel;

        return await DialogHost.Show(view) is true;
    }

    public async Task<IReadOnlyList<FileInfo>> ShowFilePickerAsync(FilePickerOpenOptions? options = null)
    {
        options ??= new() { AllowMultiple = false, Title = "Select File" };

        IReadOnlyList<IStorageFile> files = await StorageProvider.Value.OpenFilePickerAsync(options);
        return [..files.Select(x => new FileInfo(x.Path.AbsolutePath))];
    }
}