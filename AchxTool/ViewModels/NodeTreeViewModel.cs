using System.Collections.ObjectModel;
using System.ComponentModel;

using AchxTool.ViewModels.Animation;
using AchxTool.ViewModels.Nodes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels
{
    public interface INodeTree
    {
        IReadOnlyList<AchxNodeViewModel> Nodes { get; }
        public void SetSelected(AchxNodeViewModel? node);

        AchxNodeViewModel? FindParent(AchxNodeViewModel child) => Nodes.FindDirectParent(child);
        AnimationViewModel? FindAnimation(AchxNodeViewModel child) => Nodes.FindParentAnimation(child);
    }

    public partial class NodeTreeViewModel : ObservableObject, INodeTree, IRecipient<CanvasSelectionChanged>, IRecipient<ProjectLoadedMessage>
    {
        [ObservableProperty]
        private AchxNodeViewModel? _selectedNode;

        public ObservableCollection<AchxNodeViewModel> Nodes { get; } = [];
        IReadOnlyList<AchxNodeViewModel> INodeTree.Nodes => Nodes;

        private IMessenger Messenger { get; }

        public NodeTreeViewModel(IMessenger messenger)
        {
            Messenger = messenger;
            messenger.RegisterAll(this);
        }

        public void AddAnimation(AnimationViewModel animation)
        {
            Nodes.Add(animation);
        }

        public void SetSelected(AchxNodeViewModel? node)
        {
            SelectedNode = node;
        }

        partial void OnSelectedNodeChanged(AchxNodeViewModel? value)
        {
            foreach (var node in Nodes.Flatten())
            {
                node.IsSelected = node == value;
            }
            Messenger.Send<TreeNodeSelectedMessage>(new(value));
        }

        void IRecipient<CanvasSelectionChanged>.Receive(CanvasSelectionChanged message)
        {
            if (message.CanvasItem is AchxNodeViewModel node)
            {
                SelectedNode = node;
            }
        }

        void IRecipient<ProjectLoadedMessage>.Receive(ProjectLoadedMessage message)
        {
            Nodes.Clear();
            SelectedNode = null;
            foreach (AnimationViewModel node in message.Project.Animations)
            {
                Nodes.Add(node);
            }
        }

    }

    public record TreeNodeSelectedMessage(AchxNodeViewModel? Node);

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
            List<AchxNodeViewModel> allNodes = [.. nodes.Flatten()];

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

            List<AchxNodeViewModel> allNodes = [.. nodes.Flatten()];

            return allNodes.FindDirectParent(node) switch
            {
                AnimationViewModel animation => animation,
                FrameViewModel frame => allNodes.FindDirectParent(frame) as AnimationViewModel,
                _ => null
            };
        }
    }
}