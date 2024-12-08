using System.Collections.ObjectModel;

using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AchxTool.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private AchxNodeViewModel? _selectedNode;

    public ObservableCollection<AchxNodeViewModel> Nodes { get; } = [];

    public CanvasViewModel CanvasViewModel { get; }

    public MainViewModel(CanvasViewModel canvasViewModel)
    {
        CanvasViewModel = canvasViewModel;

        List<AchxNodeViewModel> nodes = [..MockData(() => new(), () => new() { Width = 50, Height = 50 })];
        foreach (var node in nodes)
        {
            AddNode(node);
        }
    }

    private void AddNode(AchxNodeViewModel node)
    {
        Nodes.Add(node);
        if (node is AnimationChainViewModel chain)
        {
            foreach (var frame in chain.Frames)
            {
                CanvasViewModel.Items.Add(frame);
            }
        }
    }

    partial void OnSelectedNodeChanged(AchxNodeViewModel? value)
    {
        foreach (var node in Nodes
                     .OfType<AnimationChainViewModel>()
                     .Aggregate(new List<AchxNodeViewModel>(),
                         (acc, x) =>
                         {
                             acc.AddRange([x, .. x.Frames]);
                             return acc;
                         }))
        {
            node.IsSelected = node == value;

            if (node.IsSelected)
            {
                CanvasViewModel.SelectedItem = node as ICanvasItem ?? null;
            }
        }
    }

    private IEnumerable<AchxNodeViewModel> MockData(Func<AnimationChainViewModel> chain, Func<AnimationFrameViewModel> frame)
    {
        AchxNodeViewModel idle = chain();
        idle.Name = "Idle";
        yield return idle;

        AnimationChainViewModel jumping = chain();
        jumping.Name = "Jumping";
        jumping.Frames.Add(frame());
        jumping.Frames.Add(frame());
        AnimationFrameViewModel peak = frame();
        peak.Name = "Peak";
        jumping.Frames.Add(peak);
        jumping.Frames.Add(frame());
        jumping.Frames.Add(frame());
        yield return jumping;

        AchxNodeViewModel walking = chain();
        walking.Name = "Walking";
        yield return walking;
    }
}
