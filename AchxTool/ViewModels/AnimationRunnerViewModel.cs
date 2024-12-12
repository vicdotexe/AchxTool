using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

using AchxTool.Services;
using AchxTool.ViewModels.Nodes;

using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

namespace AchxTool.ViewModels;

public partial class AnimationRunnerViewModel : ObservableObject, IRecipient<Messages.ActiveAnimationChanged>, IRecipient<Messages.SelectedNodeChanged>
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TotalFrames))]
    private AnimationViewModel? _activeAnimation;

    public FrameViewModel? CurrentFrame => ActiveAnimation?.Frames.ElementAtOrDefault(CurrentIndex);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentFrame))]
    [NotifyPropertyChangedFor(nameof(Image))]
    private int _currentIndex;

    [ObservableProperty] 
    private bool _isRunning = true;

    public int TotalFrames => ActiveAnimation?.Frames.Count ?? 0;

    public Bitmap? Image => CurrentFrame?.TextureName is not null ? BitmapBank.Get(CurrentFrame.TextureName) : null;

    private Stopwatch StopWatch { get; }

    private double _lastElapsed;
    private double _elapsedSinceFrameStart;

    private IBitmapBank BitmapBank { get; }

    public AnimationRunnerViewModel(IBitmapBank bitmapBank, IMessenger messenger)
    {
        BitmapBank = bitmapBank;

        DispatcherTimer timer  = new ()
        {
            Interval = TimeSpan.FromMilliseconds(16),
        };

        timer.Tick += TimerOnTick;
        timer.Start();
        StopWatch = Stopwatch.StartNew();
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

        if (CurrentFrame is not null && ActiveAnimation is not null)
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
            StopWatch.Reset();
        }
        else
        {
            _lastElapsed = 0;
            _elapsedSinceFrameStart = 0;
            StopWatch.Start();
        }
    }

    private void Restart()
    {
        CurrentIndex = 0;
        _lastElapsed = 0;
        _elapsedSinceFrameStart = 0;
        StopWatch.Restart();
    }

    void IRecipient<Messages.ActiveAnimationChanged>.Receive(Messages.ActiveAnimationChanged message)
    {
        ActiveAnimation = message.AnimationViewModel;
    }

    void IRecipient<Messages.SelectedNodeChanged>.Receive(Messages.SelectedNodeChanged message)
    {
        if (ActiveAnimation is null || IsRunning)
        {
            return;
        }

        if (message.Node is FrameViewModel frame && ActiveAnimation.Frames.IndexOf(frame) is var index and >= 0)
        {
            CurrentIndex = index;
        }
    }
}