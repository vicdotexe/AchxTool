using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels
{
    public class CanvasViewModel : ObservableObject
    {
        public ObservableCollection<CanvasItemViewModel> Items { get; } = [new CanvasBoxViewModel{Width = 50, Height = 50}];
    }
}