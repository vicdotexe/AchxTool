﻿using System.Collections.ObjectModel;

using Avalonia.Input;
using Avalonia.Xaml.Interactions.DragAndDrop;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AchxTool.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public string Greeting => "Welcome to Avalonia!";

    [ObservableProperty]
    private int _counter;

    [ObservableProperty]
    private AchxNodeViewModel? _selectedNode;

    partial void OnSelectedNodeChanged(AchxNodeViewModel? value)
    {
        foreach (var frame in Nodes.SelectMany(x =>
                 {
                     List<AchxNodeViewModel> result = [x, ..x.Frames];
                     return result;
                 }))
        {
            frame.IsSelected = false;
        }

        if (value is {IsSelected:false})
        {
            value.IsSelected = true;
        }
    }

    [RelayCommand]
    public void Increment() => Counter++;

    public ObservableCollection<AnimationChainViewModel> Nodes { get; } = [];

    public CanvasViewModel CanvasViewModel { get; } = new();

    public MainViewModel()
    {

        Nodes.Add(new()
        {
            Name = "Idle"
        });

        AnimationChainViewModel jump = new()
        {
            Name = "Jumping",
            Frames = { new(), new(), new() { Name = "Peak" }, new(), new() }
        };
        Nodes.Add(jump);

        Nodes.Add(new (){Name = "Walking"});

        foreach (var frame in Nodes.SelectMany(x => x.Frames))
        {
            frame.Width = 50;
            frame.Height = 50;
            CanvasViewModel.Items.Add(frame);
            frame.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(AchxNodeViewModel.IsSelected) && sender is AchxNodeViewModel {IsSelected: true} node)
                {
                    SelectedNode = node;
                }
            };
        }
        
    }

    [ObservableProperty]
    private IDragHandler _dragHandler = new TreeItemDragHandler();
}

public class TreeItemDragHandler : IDragHandler
{
    public void BeforeDragDrop(object? sender, PointerEventArgs e, object? context)
    {

    }

    public void AfterDragDrop(object? sender, PointerEventArgs e, object? context)
    {

    }
}