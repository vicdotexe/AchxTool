using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels;

public partial class AnimationFrameViewModel : AchxNodeViewModel
{
    [ObservableProperty]
    private bool _flipHorizontal;

    [ObservableProperty] 
    private bool _flipVertical;

    [ObservableProperty] 
    private float _frameLength;

    [ObservableProperty] 
    private string _textureName;

    [ObservableProperty]
    private float _x;

    [ObservableProperty]
    private float _y;

    [ObservableProperty]
    private float _width;

    [ObservableProperty]
    private float _height;

    public ObservableCollection<ColliderNodeViewModel> Colliders { get; } = [];
}
