using Avalonia.Controls;

using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Avalonia.Platform.Storage;

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
