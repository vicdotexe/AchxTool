using System.Collections.Specialized;
using System.Diagnostics;

using AchxTool.Services;
using AchxTool.ViewModels.Nodes;

using Avalonia.Media.Imaging;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels.Animation;

public partial class AnimationRunnerViewModel : ObservableObject, IRecipient<TreeNodeSelectedMessage>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalFrames))]
    [NotifyPropertyChangedFor(nameof(CurrentFrame))]
    [NotifyPropertyChangedFor(nameof(Image))]
    private AnimationViewModel? _activeAnimation;

    public FrameViewModel? CurrentFrame => ActiveAnimation?.Frames.ElementAtOrDefault(CurrentIndex);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentFrame))]
    [NotifyPropertyChangedFor(nameof(Image))]
    private int _currentIndex;

    [ObservableProperty]
    private bool _isRunning = true;

    public int TotalFrames => ActiveAnimation?.Frames.Count ?? 0;

    public Bitmap? Image => TextureProvider.Get(CurrentFrame?.TextureFile);

    private Stopwatch StopWatch { get; } = new();

    private double _lastElapsed;
    private double _elapsedSinceFrameStart;

    private ITextureProvider TextureProvider { get; }
    private INodeTree NodeTree { get; }

    public AnimationRunnerViewModel(ITextureProvider textureProvider, IMessenger messenger, INodeTree nodeTree)
    {
        TextureProvider = textureProvider;
        NodeTree = nodeTree;

        DispatcherTimer timer = new()
        {
            Interval = TimeSpan.FromMilliseconds(16),
        };

        timer.Tick += TimerOnTick;
        timer.Start();

        messenger.RegisterAll(this);
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        if (!IsRunning)
        {
            return;
        }
        double elapsed = (StopWatch.ElapsedMilliseconds - _lastElapsed) / 1000;
        _elapsedSinceFrameStart += elapsed;

        if (CurrentFrame is not null)
        {
            if (_elapsedSinceFrameStart >= CurrentFrame.FrameLength)
            {
                var remainder = _elapsedSinceFrameStart - CurrentFrame.FrameLength;

                CurrentIndex = CurrentIndex >= TotalFrames - 1 ? 0 : CurrentIndex + 1;
                _elapsedSinceFrameStart = remainder;
            }
        }
        _lastElapsed = StopWatch.ElapsedMilliseconds;
    }

    partial void OnActiveAnimationChanged(AnimationViewModel? oldValue, AnimationViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.Frames.CollectionChanged -= FramesOnCollectionChanged;
        }

        if (newValue is not null)
        {
            newValue.Frames.CollectionChanged += FramesOnCollectionChanged;
        }

        Restart();

        void FramesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(TotalFrames));
            Restart();
        }
    }

    partial void OnIsRunningChanged(bool value)
    {
        if (!value)
        {
            StopWatch.Stop();
            if (CurrentFrame is not null)
            {
                NodeTree.SetSelected(CurrentFrame);
            }
        }
        else
        {
            _lastElapsed = 0;
            _elapsedSinceFrameStart = 0;
            StopWatch.Reset();
            StopWatch.Start();
        }
    }

    partial void OnCurrentIndexChanged(int value)
    {
        if (!IsRunning && CurrentFrame is not null)
        {
            NodeTree.SetSelected(CurrentFrame);
        }
    }

    private void Restart()
    {
        CurrentIndex = 0;
        _lastElapsed = 0;
        _elapsedSinceFrameStart = 0;
        StopWatch.Restart();
    }

    void IRecipient<TreeNodeSelectedMessage>.Receive(TreeNodeSelectedMessage message)
    {
        if (message.Node is null)
        {
            ActiveAnimation = null;
            CurrentIndex = 0;
            return;
        }

        if (NodeTree.FindAnimation(message.Node) is { } animation)
        {
            ActiveAnimation = animation;

            if (message.Node is FrameViewModel frame)
            {
                CurrentIndex = animation.Frames.IndexOf(frame);
            }
        }
    }
}