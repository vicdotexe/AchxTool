using System.Collections.ObjectModel;

using AchxTool.Behaviors;
using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.DragAndDrop;

namespace AchxTool.Behaviors;
public class NodeTreeViewDropHandler : DropHandlerBase
{

    public override bool Validate(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (sender is TreeView treeView &&
            sourceContext is AchxNodeViewModel &&
            targetContext is MainViewModel &&
            treeView.GetVisualAt(e) is TextBlock { DataContext: AchxNodeViewModel controlContext } control &&
            !ReferenceEquals(sourceContext, controlContext))
        {
            return true;
        };
        return false;
    }

    public override void Over(object? sender, DragEventArgs e, object? sourceContext, object? targetContext)
    {
        base.Over(sender, e, sourceContext, targetContext);

    }

    public override bool Execute(object? sender, DragEventArgs e, object? sourceContext, object? targetContext, object? state)
    {
        if (e.Source is Control && sender is TreeView treeView)
        {
            return Validate<AchxNodeViewModel>(treeView, e, sourceContext, targetContext, true);
        }
        return false;
    }

    private bool Validate<T>(TreeView treeView, DragEventArgs e, object? sourceContext, object? targetContext, bool bExecute) where T : AchxNodeViewModel
    {
        if (sourceContext is not T sourceNode
            || targetContext is not MainViewModel vm)
        {
            return false;
        }

        if (treeView.GetVisualAt(e.GetPosition(treeView)) is not Control targetControl
            || targetControl.DataContext is not T targetNode)
        {
            return false;
        }

        if (sourceContext is AnimationChainViewModel sourceChain && targetNode is AnimationChainViewModel targetChain)
        {
            SwapItem(vm.Nodes, treeView.Items.IndexOf(sourceChain), treeView.Items.IndexOf(targetChain));
            return true;
        }
        //var sourceParent = sourceNode.Parent;
        //var targetParent = targetNode.Parent;
        //var sourceNodes = sourceParent is not null ? sourceParent.Nodes : vm.Nodes;
        //var targetNodes = targetParent is not null ? targetParent.Nodes : vm.Nodes;

        //if (sourceNodes is not null && targetNodes is not null)
        //{
        //    var sourceIndex = sourceNodes.IndexOf(sourceNode);
        //    var targetIndex = targetNodes.IndexOf(targetNode);

        //    if (sourceIndex < 0 || targetIndex < 0)
        //    {
        //        return false;
        //    }

        //    switch (e.DragEffects)
        //    {
        //        case DragDropEffects.Copy:
        //            {
        //                if (bExecute)
        //                {
        //                    var clone = new NodeViewModel() { Title = sourceNode.Title + "_copy" };
        //                    InsertItem(targetNodes, clone, targetIndex + 1);
        //                }

        //                return true;
        //            }
        //        case DragDropEffects.Move:
        //            {
        //                if (bExecute)
        //                {
        //                    if (sourceNodes == targetNodes)
        //                    {
        //                        MoveItem(sourceNodes, sourceIndex, targetIndex);
        //                    }
        //                    else
        //                    {
        //                        sourceNode.Parent = targetParent;

        //                        MoveItem(sourceNodes, targetNodes, sourceIndex, targetIndex);
        //                    }
        //                }

        //                return true;
        //            }
        //        case DragDropEffects.Link:
        //            {
        //                if (bExecute)
        //                {
        //                    if (sourceNodes == targetNodes)
        //                    {
        //                        SwapItem(sourceNodes, sourceIndex, targetIndex);
        //                    }
        //                    else
        //                    {
        //                        sourceNode.Parent = targetParent;
        //                        targetNode.Parent = sourceParent;

        //                        SwapItem(sourceNodes, targetNodes, sourceIndex, targetIndex);
        //                    }
        //                }

        //                return true;
        //            }
        //    }
        //}

        return false;
    }
}

file static class Helpers
{
    public static Visual? GetVisualAt(this Control control, DragEventArgs e)
    {
        return control.GetVisualAt(e.GetPosition(control));
    }
}