using System.Collections.ObjectModel;

using AchxTool.Services;
using AchxTool.ViewModels.Animation;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels.Nodes;

public partial class FrameViewModel : AchxNodeViewModel, ICanvasItem, IHaveTexture
{
    [ObservableProperty]
    private bool _flipHorizontal;

    [ObservableProperty]
    private bool _flipVertical;

    [ObservableProperty]
    private double _frameLength;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Texture))]
    private FileInfo? _textureFile;

    public TextureViewModel? Texture =>
        TextureFile is { } fileInfo ? TextureManager.Get(fileInfo) : ParentAnimation.Texture;

    [ObservableProperty]
    private double _x;

    [ObservableProperty]
    private double _y;

    [ObservableProperty]
    private double _z;

    [ObservableProperty]
    private double _width;

    [ObservableProperty]
    private double _height;

    [ObservableProperty]
    private bool _isDragEnabled = true;

    [ObservableProperty]
    private bool _isResizeEnabled = true;

    [ObservableProperty]
    private bool _isSelectionEnabled = true;

    public AnimationViewModel ParentAnimation { get; }
    private IMessenger Messenger { get; }
    private ITextureManager TextureManager { get; }

    public FrameViewModel(AnimationViewModel parentAnimation, IMessenger messenger, ITextureManager textureManager)
    {
        ParentAnimation = parentAnimation;
        ParentAnimation.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(ParentAnimation.TextureFile))
            {
                if (TextureFile is null)
                {
                    OnPropertyChanged(nameof(Texture));
                }
            }
        };

        Messenger = messenger;
        TextureManager = textureManager;
    }

    public ObservableCollection<ColliderNodeViewModel> Colliders { get; } = [];

    partial void OnTextureFileChanged(FileInfo? value)
    {
        Messenger.Send<NodeTextureChangedMessage>(new(this));
    }
}

public record class NodeTextureChangedMessage(AchxNodeViewModel Node);