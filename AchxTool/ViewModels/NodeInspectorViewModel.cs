using System.Collections.ObjectModel;

using AchxTool.Services;
using AchxTool.ViewModels.Nodes;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels;

public partial class NodeInspectorViewModel : ObservableObject
{
    [ObservableProperty] 
    public AchxNodeViewModel? _selectedNode;

    private ITextureProvider TextureProvider { get; }

    public NodeInspectorViewModel(ITextureProvider textureProvider)
    {
        TextureProvider = textureProvider;
    }
}