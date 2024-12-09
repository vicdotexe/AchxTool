using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;

using Avalonia.Controls;
using Avalonia.Media.Imaging;
using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels;

public partial class AnimationRunnerViewModel : ObservableObject
{
    [ObservableProperty]
    private AnimationChainViewModel? _activeChain;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Image))]
    private AnimationFrameViewModel? _currentFrame;

    private Stopwatch StopWatch { get; }

    private double _lastElapsed;
    private double _elapsedSinceFrameStart;

    private int CurrentIndex;
    private int TotalFrames;

    public Bitmap? Image => CurrentFrame?.TextureName is not null ? BitmapBank.Get(CurrentFrame.TextureName) : null;

    partial void OnActiveChainChanged(AnimationChainViewModel? oldValue, AnimationChainViewModel? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.Frames.CollectionChanged -= FramesOnCollectionChanged;
        }

        if (newValue is not null)
        {
            newValue.Frames.CollectionChanged += FramesOnCollectionChanged;
            TotalFrames = newValue.Frames.Count;
            CurrentIndex = 0;
            CurrentFrame = newValue.Frames.FirstOrDefault();
        }
    }

    private void FramesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        TotalFrames = (sender as IList)?.Count ?? 0;
        if (CurrentIndex >= TotalFrames)
        {
            CurrentIndex = 0;
        }
    }

    private IBitmapBank BitmapBank { get; }

    public AnimationRunnerViewModel(IBitmapBank bitmapBank)
    {
        BitmapBank = bitmapBank;

        StopWatch = Stopwatch.StartNew();
        var timer  = new DispatcherTimer()
        {
            Interval = TimeSpan.FromMilliseconds(16),
        };
        timer.Tick += TimerOnTick;
        timer.Start();
    }

    
    private void TimerOnTick(object? sender, EventArgs e)
    {
        double elapsed = StopWatch.ElapsedMilliseconds - _lastElapsed;
        _elapsedSinceFrameStart += elapsed;

        if (CurrentFrame is not null && ActiveChain is not null)
        {
            if (_elapsedSinceFrameStart >= CurrentFrame.FrameLength)
            {
                var remainder = _elapsedSinceFrameStart - CurrentFrame.FrameLength;

                CurrentIndex++;
                if (CurrentIndex >= TotalFrames)
                {
                    CurrentIndex = 0;
                }

                CurrentFrame = ActiveChain.Frames[CurrentIndex];
                _elapsedSinceFrameStart = remainder;
            }
        }

        _lastElapsed = StopWatch.ElapsedMilliseconds;
    }
}