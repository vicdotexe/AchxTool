using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AchxTool.Models;
using AchxTool.ViewModels.Nodes;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels;

public partial class FrameViewModel : AchxNodeViewModel, ICanvasItem
{
    [ObservableProperty]
    private bool _flipHorizontal;

    [ObservableProperty] 
    private bool _flipVertical;

    [ObservableProperty] 
    private double _frameLength;

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
    private bool _isResizeEnabled = true;

    [ObservableProperty]
    private bool _isSelectionEnabled = true;

    public ObservableCollection<ColliderNodeViewModel> Colliders { get; } = [];

    public void Load(FrameModel model)
    {


    }
}
