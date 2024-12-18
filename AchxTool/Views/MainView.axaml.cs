using AchxTool.ViewModels;

using Avalonia.Controls;

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
}