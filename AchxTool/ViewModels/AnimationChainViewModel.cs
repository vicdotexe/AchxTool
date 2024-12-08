using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels;

public class AnimationChainViewModel : AchxNodeViewModel
{
    public ObservableCollection<AnimationFrameViewModel> Frames { get; } = [];

    public void AddFrameCommand()
    {
        Frames.Add(new());
    }
}