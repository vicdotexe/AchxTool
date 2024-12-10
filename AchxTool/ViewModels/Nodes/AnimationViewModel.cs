using System.Collections.ObjectModel;
using System.Collections.Specialized;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels.Nodes;

public class AnimationViewModel : AchxNodeViewModel
{
    public ObservableCollection<FrameViewModel> Frames { get; } = [];
    private Func<FrameViewModel> FrameFactory { get; }
    public AnimationViewModel(Func<FrameViewModel> frameFactory)
    {
        FrameFactory = frameFactory;
    }

    public void AddFrameCommand()
    {
        FrameViewModel frame = FrameFactory();

        frame.Width = 50;
        frame.Height = 50;

        if (Frames.LastOrDefault() is { } lastFrame)
        {
            frame.X = lastFrame.X + lastFrame.Width;
            frame.Y = lastFrame.Y;

            frame.Width = lastFrame.Width;
            frame.Height = lastFrame.Height;
            frame.FrameLength = lastFrame.FrameLength;
            frame.TextureName = lastFrame.TextureName;
        }

        Frames.Add(frame);
    }
}