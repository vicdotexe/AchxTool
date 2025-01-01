using System.Collections.ObjectModel;

using AchxTool.Services;
using AchxTool.ViewModels.Nodes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels;

public partial class NodeInspectorViewModel : ObservableObject, IRecipient<TreeNodeSelectedMessage>
{
    [ObservableProperty] 
    private AchxNodeViewModel? _selectedNode;

    public ITextureManager TextureManager { get; }

    public NodeInspectorViewModel(ITextureManager textureManager, IMessenger messenger)
    {
        TextureManager = textureManager;
        messenger.RegisterAll(this);
    }

    void IRecipient<TreeNodeSelectedMessage>.Receive(TreeNodeSelectedMessage message)
    {
        SelectedNode = message.Node;
    }
}