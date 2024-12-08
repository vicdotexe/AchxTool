using Avalonia.Controls;

using AchxTool.ViewModels;

namespace AchxTool.Views;

public partial class MainView : UserControl
{
    // This constructor is used when the view is created by the XAML Previewer
    public MainView()
    {
        InitializeComponent();
    }

    // This constructor is used when the view is created via dependency injection
    public MainView(MainViewModel viewModel)
        : this()
    {
        DataContext = viewModel;
    }

    private void MainTreeView_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (DataContext is MainViewModel vm)
        {
            if (e.AddedItems is [ICanvasItem canvasItem, ..])
            {
                vm.CanvasViewModel.SelectedItem = canvasItem;
            }
        }
    }
}
