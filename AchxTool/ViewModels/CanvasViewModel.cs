using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels
{
    public class CanvasViewModel : ObservableObject
    {
        public ObservableCollection<AnimationFrameViewModel> Items { get; } = [];
    }
}