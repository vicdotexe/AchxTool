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

    public static readonly StyledProperty<double> ZoomXProperty =
        AvaloniaProperty.Register<MainView, double>(nameof(ZoomX), 1.0);

    public static readonly StyledProperty<double> ZoomYProperty =
        AvaloniaProperty.Register<MainView, double>(nameof(ZoomY), 1.0);

    public double ZoomX
    {
        get => GetValue(ZoomXProperty);
        set => SetValue(ZoomXProperty, value);
    } 

    public double ZoomY
    {
        get => GetValue(ZoomYProperty);
        set => SetValue(ZoomYProperty, value);
    }

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

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        this.FindDescendantOfType<ZoomBorder>().ZoomChanged += MainView_ZoomChanged;
    }

    private void MainView_ZoomChanged(object sender, ZoomChangedEventArgs e)
    {
        ZoomX = e.ZoomX;
        ZoomY = e.ZoomY;
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

    private async void MenuItem_Load_OnClick(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open Text File",
            AllowMultiple = false
        });

        if (files.FirstOrDefault() is { } file && DataContext is MainViewModel vm)
        {
            vm.LoadProject(file.Path.AbsolutePath);
        }
    }
}
