using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels;

public partial class AnimationFrameViewModel : AchxNodeViewModel, ICanvasItem
{
    [ObservableProperty]
    private bool _flipHorizontal;

    [ObservableProperty] 
    private bool _flipVertical;

    [ObservableProperty] 
    private float _frameLength;

    [ObservableProperty] 
    private string? _textureName;

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
    private bool _isResizeEnabled;

    public ObservableCollection<ColliderNodeViewModel> Colliders { get; } = [];

    public void Select()
    {
        IsSelected = true;
    }

    double? ICanvasItem.Width
    {
        get => Width >= 0 ? Width : null;
        set => Width = value ?? -1;
    }    
    
    double? ICanvasItem.Height
    {
        get => Height >= 0 ? Height : null;
        set => Height = value ?? -1;
    }
}
