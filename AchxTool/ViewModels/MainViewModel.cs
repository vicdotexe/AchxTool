using System.Collections.ObjectModel;

using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AchxTool.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ActiveAnimation))]
    private AchxNodeViewModel? _selectedNode;

    public AnimationChainViewModel? ActiveAnimation =>
        SelectedNode is not null ? Nodes.FindParentAnimation(SelectedNode) : null;

    public ObservableCollection<AchxNodeViewModel> Nodes { get; } = [];

    public CanvasViewModel CanvasViewModel { get; }
    public AnimationRunnerViewModel AnimationRunner { get; }

    public MainViewModel(CanvasViewModel canvasViewModel, 
        Func<AnimationChainViewModel> animationFactory, 
        Func<AnimationFrameViewModel> frameFactory, 
        AnimationRunnerViewModel animationRunner)
    {
        CanvasViewModel = canvasViewModel;
        AnimationRunner = animationRunner;

        List<AchxNodeViewModel> nodes = [..MockData(animationFactory, () =>
        {
            var frame = frameFactory();
            frame.Width = 50;
            frame.Height = 50;
            frame.FrameLength = 250;
            frame.TextureName = "test-spritesheet.png";
            return frame;
        })];
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
        foreach (var node in Nodes.Flatten())
        {
            node.IsSelected = node == value;
        }

        CanvasViewModel.SelectedItem = value as ICanvasItem;
        AnimationRunner.ActiveChain = ActiveAnimation;
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

public static class NodeHelpers
{
    public static IEnumerable<AchxNodeViewModel> Flatten(this IEnumerable<AchxNodeViewModel> nodes)
    {
        foreach (var node in nodes)
        {
            if (node is AnimationChainViewModel chain)
            {
                yield return chain;
                foreach (var frame in chain.Frames)
                {
                    yield return frame;
                    foreach (var collider in frame.Colliders)
                    {
                        yield return collider;
                    }
                }
            }
            else
            {
                yield return node;
            }
        }
    }

    public static AchxNodeViewModel? FindDirectParent(this IEnumerable<AchxNodeViewModel> nodes, AchxNodeViewModel node)
    {
        List<AchxNodeViewModel> allNodes = [..nodes.Flatten()];

        return allNodes.FirstOrDefault(n => n switch
        {
            AnimationChainViewModel chain => chain.Frames.Contains(node),
            AnimationFrameViewModel frame => frame.Colliders.Contains(node),
            ColliderNodeViewModel collider => false,
            _ => false
        });
    }

    public static AnimationChainViewModel? FindParentAnimation(this IEnumerable<AchxNodeViewModel> nodes,
        AchxNodeViewModel node)
    {
        List<AchxNodeViewModel> allNodes = [..nodes.Flatten()];

        return allNodes.FindDirectParent(node) switch
        {
            AnimationChainViewModel chain => chain,
            AnimationFrameViewModel frame => allNodes.FindDirectParent(frame) as AnimationChainViewModel,
            _ => null
        };
    }

}
