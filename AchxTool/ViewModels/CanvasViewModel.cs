using System.Collections.ObjectModel;
using System.Collections.Specialized;

using AchxTool.Services;
using AchxTool.ViewModels.Nodes;

using Avalonia.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels;

public partial class CanvasViewModel : ObservableObject, IRecipient<Messages.SelectedNodeChanged>,
    IRecipient<Messages.ActiveAnimationChanged>
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
        TextureViewModel.ImageSource = value is FrameViewModel frame ? frame.TextureName : null;
        if (value is AchxNodeViewModel node)
        {
            Messenger.Send<Messages.CanvasSelectedNewNode>(new(node));
        }


        foreach (var item in Items.Where(x => x is not CanvasTextureViewModel))
        {
            item.Z = item == value ? 1 : 0;
        }
    }

    void IRecipient<Messages.SelectedNodeChanged>.Receive(Messages.SelectedNodeChanged message)
    {
        SelectedItem = message.Node as ICanvasItem;
    }

    void IRecipient<Messages.ActiveAnimationChanged>.Receive(Messages.ActiveAnimationChanged message)
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

public partial class Messages
{
    public record CanvasSelectedNewNode(AchxNodeViewModel Node);
}



