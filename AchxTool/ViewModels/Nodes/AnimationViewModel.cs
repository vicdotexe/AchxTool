using System.Collections.ObjectModel;

using AchxTool.Services;
using AchxTool.ViewModels.Animation;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AchxTool.ViewModels.Nodes;

public partial class AnimationViewModel : AchxNodeViewModel, IHaveTexture
{
    public ObservableCollection<FrameViewModel> Frames { get; } = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Texture))]
    private FileInfo? _textureFile;

    public TextureViewModel? Texture => TextureManager.Get(TextureFile);

    private Func<AnimationViewModel, FrameViewModel> FrameFactory { get; }
    private ITextureManager TextureManager { get; }
    public AnimationViewModel(Func<AnimationViewModel, FrameViewModel> frameFactory, ITextureManager textureManager)
    {
        FrameFactory = frameFactory;
        TextureManager = textureManager;
    }

    [RelayCommand]
    public void AddFrame()
    {
        FrameViewModel frame = FrameFactory(this);

        frame.Width = 50;
        frame.Height = 50;

        if (Frames.LastOrDefault() is { } lastFrame)
        {
            frame.X = lastFrame.X + lastFrame.Width;
            frame.Y = lastFrame.Y;

            frame.Width = lastFrame.Width;
            frame.Height = lastFrame.Height;
            frame.FrameLength = lastFrame.FrameLength;
            frame.TextureFile = lastFrame.TextureFile;
        }

        Frames.Add(frame);
    }
}