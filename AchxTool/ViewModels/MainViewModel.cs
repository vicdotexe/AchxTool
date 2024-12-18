﻿using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using AchxTool.Services;
using AchxTool.ViewModels.Nodes;

using Avalonia;
using Avalonia.Controls;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public Animation.CanvasViewModel CanvasViewModel { get; }
    public NodeTreeViewModel NodeTreeViewModel { get; }
    public Animation.AnimationRunnerViewModel AnimationRunner { get; }
    private IViewModelFactory Factory { get; }
    private IProjectLoader ProjectLoader { get; }
    private IDialogService DialogService { get; }

    public MainViewModel(Animation.CanvasViewModel canvasViewModel, 
        IViewModelFactory factory,
        Animation.AnimationRunnerViewModel animationRunner,
        NodeTreeViewModel nodeTreeViewModel,
        IMessenger messenger,
        IProjectLoader projectLoader,
        IDialogService dialogService)
    {
        CanvasViewModel = canvasViewModel;
        NodeTreeViewModel = nodeTreeViewModel;
        AnimationRunner = animationRunner;
        Factory = factory;
        ProjectLoader = projectLoader;
        DialogService = dialogService;

        foreach (AnimationViewModel animation in MockData())
        {
            NodeTreeViewModel.AddAnimation(animation);
        }
    }

    public async Task Save()
    {
        _ = await DialogService.ShowAsync<TestDialogViewModel>();
    }

    private IEnumerable<AnimationViewModel> MockData()
    {
        Func<AnimationViewModel> chain = Factory.NewFactory<AnimationViewModel>();

        const int frameWidth = 128 / 4;
        const int frameHeight = 128 / 2;

        Func<FrameViewModel> frame = Factory.NewFactory<FrameViewModel>(x =>
        {
            x.Width = frameWidth;
            x.Height = frameHeight;
            x.TextureName = "test-spritesheet.png";
            x.FrameLength = 0.15;
        });

        AnimationViewModel idle = chain();
        idle.Name = "Jumping";
        yield return idle;

        AnimationViewModel jumping = chain();
        jumping.Name = "Idle";


        int framesPerRow = 4;
        for (int i = 0; i < 8; i++)
        {
            FrameViewModel currentFrame = frame();
            currentFrame.X = (i % framesPerRow) * frameWidth;
            currentFrame.Y = (i / framesPerRow) * frameHeight;
            jumping.Frames.Add(currentFrame);
        }

        yield return jumping;

        AnimationViewModel walking = chain();
        walking.Name = "Walking";
        yield return walking;
    }


    public async Task LoadProject()
    {
        if (await DialogService.ShowFilePickerAsync() is not [var fileInfo])
        {
            return;
        }

        ProjectViewModel? project = await ProjectLoader.LoadProjectAsync(fileInfo.FullName);
        if (project is not null)
        {
            ProjectLoader.CurrentProject = project;
        }
    }
}


public record ActiveAnimationChangedMessage(AnimationViewModel? AnimationViewModel);
public record ProjectLoadedMessage(ProjectViewModel Project);