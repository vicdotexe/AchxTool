using System.Collections.ObjectModel;

using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AchxTool.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public string Greeting => "Welcome to Avalonia!";

    [ObservableProperty]
    private int _counter;

    [ObservableProperty]
    private AchxNodeViewModel? _selectedNode;

    [RelayCommand]
    public void Increment() => Counter++;

    public ObservableCollection<AnimationChainViewModel> Nodes { get; } = [];

    public CanvasViewModel CanvasViewModel { get; } = new();

    public MainViewModel()
    {

        Nodes.Add(new()
        {
            Name = "Idle"
        });

        AnimationChainViewModel jump = new()
        {
            Name = "Jumping",
            Frames = { new(), new(), new() { Name = "Peak" }, new(), new() }
        };
        Nodes.Add(jump);

        Nodes.Add(new (){Name = "Walking"});
    }

    [ObservableProperty]
    private IDragHandler _dragHandler = new TreeItemDragHandler();
}

public class TreeItemDragHandler : IDragHandler
{
    public void BeforeDragDrop(object? sender, PointerEventArgs e, object? context)
    {

    }

    public void AfterDragDrop(object? sender, PointerEventArgs e, object? context)
    {

    }
}