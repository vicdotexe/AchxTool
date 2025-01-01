using System.Collections.ObjectModel;
using System.Collections.Specialized;

using AchxTool.Services;
using AchxTool.ViewModels.Nodes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels.Animation;

public partial class CanvasViewModel : ObservableObject, IRecipient<TreeNodeSelectedMessage>, IRecipient<NodeTextureChangedMessage>
{
    public ObservableCollection<ICanvasItem> Items { get; } = [];

    public CanvasTextureViewModel TextureViewModel { get; }

    [ObservableProperty]
    private ICanvasItem? _selectedItem;

    private AnimationViewModel? _activeAnimation;

    private AnimationViewModel? ActiveAnimation
    {
        get => _activeAnimation;
        set
        {
            if (value != _activeAnimation)
            {
                OnActiveAnimationChanging(_activeAnimation, value);
                _activeAnimation = value;
            }
        }
    }

    private IMessenger Messenger { get; }
    private INodeTree NodeTree { get; }

    public CanvasViewModel(Func<CanvasTextureViewModel> textureVmFactory, IMessenger messenger, INodeTree nodeTree)
    {
        CanvasTextureViewModel textureVm = textureVmFactory();
        textureVm.Z = -1;
        TextureViewModel = textureVm;
        NodeTree = nodeTree;

        Items.Add(TextureViewModel);
        messenger.RegisterAll(this);
        Messenger = messenger;
    }

    partial void OnSelectedItemChanged(ICanvasItem? value)
    {
        if (value is IHaveTexture hasTexture)
        {
            TextureViewModel.Bitmap = hasTexture.Texture?.Bitmap;
        }

        foreach (var item in Items.Where(x => x is AchxNodeViewModel))
        {
            item.Z = item == value ? 1 : 0;
        }

        Messenger.Send<CanvasSelectionChanged>(new(value));
    }

    void IRecipient<TreeNodeSelectedMessage>.Receive(TreeNodeSelectedMessage message)
    {
        ActiveAnimation = message.Node switch
        {
            null => null,
            _ => NodeTree.FindAnimation(message.Node)
        };

        SelectedItem = message.Node as ICanvasItem;
    }

    void IRecipient<NodeTextureChangedMessage>.Receive(NodeTextureChangedMessage message)
    {
        if (message.Node == SelectedItem && message.Node is IHaveTexture hasTexture)
        {
            TextureViewModel.Bitmap = hasTexture.Texture?.Bitmap;
        }
    }

    private void OnActiveAnimationChanging(AnimationViewModel? oldValue, AnimationViewModel? newValue)
    {
        Items.Clear();
        Items.Add(TextureViewModel);

        if (oldValue is not null)
        {
            oldValue.Frames.CollectionChanged -= Animation_FramesChanged;
        }

        if (newValue is not null)
        {
            foreach (FrameViewModel frame in newValue.Frames)
            {
                Items.Add(frame);
            }

            newValue.Frames.CollectionChanged += Animation_FramesChanged;
        }

        TextureViewModel.Bitmap = newValue?.Frames.FirstOrDefault()?.Texture?.Bitmap ?? newValue?.Texture?.Bitmap;

        void Animation_FramesChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            foreach (var frame in e.NewItems?.Cast<FrameViewModel>() ?? [])
            {
                Items.Add(frame);
            }

            foreach (var frame in e.OldItems?.Cast<FrameViewModel>() ?? [])
            {
                Items.Remove(frame);
            }
        }
    }
}

public record CanvasSelectionChanged(ICanvasItem? CanvasItem);