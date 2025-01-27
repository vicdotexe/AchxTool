﻿using AchxTool.Services;
using AchxTool.ViewModels.Animation;
using AchxTool.ViewModels.Nodes;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels;

public partial class MainViewModel : ObservableObject
{
    public CanvasViewModel CanvasViewModel { get; }
    public NodeTreeViewModel NodeTreeViewModel { get; }
    public NodeInspectorViewModel NodeInspectorViewModel { get; }
    public AnimationRunnerViewModel AnimationRunner { get; }
    public TextureManagerViewModel TextureManagerViewModel { get; }
    private IViewModelFactory Factory { get; }
    private IProjectLoader ProjectLoader { get; }
    private IDialogService DialogService { get; }

    public MainViewModel(CanvasViewModel canvasViewModel,
        IViewModelFactory factory,
        AnimationRunnerViewModel animationRunner,
        NodeTreeViewModel nodeTreeViewModel,
        TextureManagerViewModel textureManagerViewModel,
        NodeInspectorViewModel nodeInspectorViewModel,
        IProjectLoader projectLoader,
        IDialogService dialogService,
        ITextureManager textureManager)
    {
        CanvasViewModel = canvasViewModel;
        NodeTreeViewModel = nodeTreeViewModel;
        AnimationRunner = animationRunner;
        NodeInspectorViewModel = nodeInspectorViewModel;
        TextureManagerViewModel = textureManagerViewModel;
        Factory = factory;
        ProjectLoader = projectLoader;
        DialogService = dialogService;

        foreach (AnimationViewModel animation in MockData())
        {
            foreach (FileInfo? textureFile in animation.Frames.Select(x => x.TextureFile))
            {
                if (textureFile is not null)
                {
                    textureManager.Load(textureFile);
                }
            }
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
            x.TextureFile = new("test-spritesheet.png");
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
public record ProjectLoadedMessage(ProjectViewModel Project);