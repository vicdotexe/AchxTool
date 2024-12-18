using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using AchxTool.Services;
using AchxTool.ViewModels.Nodes;

using Avalonia;
using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels;

public interface INodeTree
{
    IReadOnlyList<AchxNodeViewModel> Nodes { get; }
    AchxNodeViewModel? FindParent(AchxNodeViewModel child) => Nodes.FindDirectParent(child);
    AnimationViewModel? FindAnimation(AchxNodeViewModel child) => Nodes.FindParentAnimation(child);
}

public partial class MainViewModel : ObservableObject, INodeTree, IRecipient<CanvasSelectedNewNodeMessage>, IRecipient<ProjectLoadedMessage>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ActiveAnimation))]
    private AchxNodeViewModel? _selectedNode;

    [ObservableProperty]
    public AnimationViewModel? _activeAnimation;

    public ObservableCollection<AchxNodeViewModel> Nodes { get; } = [];
    IReadOnlyList<AchxNodeViewModel> INodeTree.Nodes => Nodes;

    public CanvasViewModel CanvasViewModel { get; }
    public AnimationRunnerViewModel AnimationRunner { get; }
    private IViewModelFactory Factory { get; }
    private IMessenger Messenger { get; }
    private IProjectLoader ProjectLoader { get; }
    private IDialogService DialogService { get; }

    public double ZoomX { get; private set; }
    public double ZoomY { get; private set; }

    public void SetZoom(double x, double y)
    {
        ZoomX = x;
        ZoomY = y;
        OnPropertyChanged(nameof(ZoomX));
        OnPropertyChanged(nameof(ZoomY));
    }

    public MainViewModel(CanvasViewModel canvasViewModel, 
        IViewModelFactory factory,
        AnimationRunnerViewModel animationRunner,
        IMessenger messenger,
        IProjectLoader projectLoader,
        IDialogService dialogService)
    {
        CanvasViewModel = canvasViewModel;
        Factory = factory;
        AnimationRunner = animationRunner;
        Messenger = messenger;
        ProjectLoader = projectLoader;
        DialogService = dialogService;

        messenger.RegisterAll(this);

        foreach (AnimationViewModel animation in MockData())
        {
            AddAnimation(animation);
        }
    }

    public void AddAnimation(AnimationViewModel animation)
    {
        Nodes.Add(animation);
    }

    partial void OnSelectedNodeChanged(AchxNodeViewModel? value)
    {
        foreach (var node in Nodes.Flatten())
        {
            node.IsSelected = node == value;
        }
        ActiveAnimation = value is not null ? Nodes.FindParentAnimation(value) : null;
        Messenger.Send<SelectedNodeChangedMessage>(new(value));
    }

    partial void OnActiveAnimationChanged(AnimationViewModel? value)
    {
        Messenger.Send<ActiveAnimationChangedMessage>(new(value));
    }

    public async void Save()
    {
        _ = await DialogService.ShowAsync<TestDialogViewModel>();
    }

    private IEnumerable<AnimationViewModel> MockData()
    {
        Func<AnimationViewModel> chain = Factory.NewFactory<AnimationViewModel>();

        const int frameWidth = 128 / 4;
        const int frameHeight = 128 / 2;

        Func<FrameViewModel> frame = Factory.NewFactory<FrameViewModel>(x =>
        {
            x.Width = frameWidth;
            x.Height = frameHeight;
            x.TextureName = "test-spritesheet.png";
            x.FrameLength = 0.15;
        });

        AnimationViewModel idle = chain();
        idle.Name = "Jumping";
        yield return idle;

        AnimationViewModel jumping = chain();
        jumping.Name = "Idle";


        int framesPerRow = 4;
        for (int i = 0; i < 8; i++)
        {
            FrameViewModel currentFrame = frame();
            currentFrame.X = (i % framesPerRow) * frameWidth;
            currentFrame.Y = (i / framesPerRow) * frameHeight;
            jumping.Frames.Add(currentFrame);
        }

        yield return jumping;

        AnimationViewModel walking = chain();
        walking.Name = "Walking";
        yield return walking;
    }

    void IRecipient<CanvasSelectedNewNodeMessage>.Receive(CanvasSelectedNewNodeMessage message)
    {
        SelectedNode = message.Node;
    }

    void IRecipient<ProjectLoadedMessage>.Receive(ProjectLoadedMessage message)
    {
        Nodes.Clear();
        foreach (AnimationViewModel node in message.Project.Animations)
        {
            Nodes.Add(node);
        }
    }

    public async void LoadProject(string filePath)
    {
        ProjectViewModel? project = await ProjectLoader.LoadProjectAsync(filePath);
        if (project is not null)
        {
            ProjectLoader.CurrentProject = project;
        }
    }


}
public record SelectedNodeChangedMessage(AchxNodeViewModel? Node);
public record ActiveAnimationChangedMessage(AnimationViewModel? AnimationViewModel);
public record ProjectLoadedMessage(ProjectViewModel Project);


public static class NodeHelpers
{
    public static IEnumerable<AchxNodeViewModel> Flatten(this IEnumerable<AchxNodeViewModel> nodes)
    {
        foreach (var node in nodes)
        {
            if (node is AnimationViewModel chain)
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
            AnimationViewModel chain => chain.Frames.Contains(node),
            FrameViewModel frame => frame.Colliders.Contains(node),
            _ => false
        });
    }

    public static AnimationViewModel? FindParentAnimation(this IEnumerable<AchxNodeViewModel> nodes,
        AchxNodeViewModel node, bool includeSelf = true)
    {
        if (node is AnimationViewModel self && includeSelf)
        {
            return self;
        }

        List<AchxNodeViewModel> allNodes = [..nodes.Flatten()];

        return allNodes.FindDirectParent(node) switch
        {
            AnimationViewModel animation => animation,
            FrameViewModel frame => allNodes.FindDirectParent(frame) as AnimationViewModel,
            _ => null
        };
    }
}


