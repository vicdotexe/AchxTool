using System.Collections.ObjectModel;
using System.Collections.Specialized;

using AchxTool.ViewModels.Nodes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels.Animation;

public partial class CanvasViewModel : ObservableObject, IRecipient<TreeNodeSelectedMessage>,
    IRecipient<ActiveAnimationChangedMessage>
{
    public ObservableCollection<ICanvasItem> Items { get; } = [];

    public CanvasTextureViewModel TextureViewModel { get; }

    [ObservableProperty] private ICanvasItem? _selectedItem;
    private AnimationViewModel? ActiveAnimation { get; set; }

    private IMessenger Messenger { get; }

    public CanvasViewModel(Func<CanvasTextureViewModel> textureVmFactory, IMessenger messenger)
    {
        var textureVm = textureVmFactory();
        textureVm.Z = -1;
        TextureViewModel = textureVm;

        Items.Add(TextureViewModel);
        messenger.RegisterAll(this);
        Messenger = messenger;
    }

    partial void OnSelectedItemChanged(ICanvasItem? value)
    {
        if (value is FrameViewModel frame && TextureViewModel.ImageSource != frame.TextureName)
        {
            TextureViewModel.ImageSource = frame.TextureName;
        }

        foreach (var item in Items.Except([TextureViewModel]))
        {
            item.Z = item == value ? 1 : 0;
        }

        Messenger.Send<CanvasSelectionChanged>(new(value));
    }

    void IRecipient<TreeNodeSelectedMessage>.Receive(TreeNodeSelectedMessage message)
    {
        SelectedItem = message.Node as ICanvasItem;
    }

    void IRecipient<ActiveAnimationChangedMessage>.Receive(ActiveAnimationChangedMessage message)
    {
        Items.Clear();
        Items.Add(TextureViewModel);

        if (ActiveAnimation is not null)
        {
            ActiveAnimation.Frames.CollectionChanged -= Animation_FramesChanged;
        }

        if (message.AnimationViewModel is { } animation)
        {
            foreach (FrameViewModel frame in animation.Frames)
            {
                Items.Add(frame);
            }

            animation.Frames.CollectionChanged += Animation_FramesChanged;
        }

        ActiveAnimation = message.AnimationViewModel;
        TextureViewModel.ImageSource = ActiveAnimation?.Frames.FirstOrDefault()?.TextureName;

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


