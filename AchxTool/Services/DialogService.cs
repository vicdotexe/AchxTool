using AchxTool.ViewModels;
using Avalonia.Controls;
using DialogHostAvalonia;

namespace AchxTool.Services;
public interface IDialogService
{
    Task<bool> ShowAsync<TDialogViewModel>(TDialogViewModel? viewModel = null)
        where TDialogViewModel : DialogViewModelBase;
}
public class DialogService : IDialogService
{
    private IViewModelFactory ViewModelFactory { get; }


    public DialogService(IViewModelFactory viewModelFactory)
    {
        ViewModelFactory = viewModelFactory;
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
}