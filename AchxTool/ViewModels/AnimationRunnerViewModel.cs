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

public partial class AnimationRunnerViewModel : ObservableObject, IRecipient<Messages.ActiveAnimationChanged>
{
    [ObservableProperty]
    private AnimationViewModel? _activeAnimation;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(Image))]
    private FrameViewModel? _currentFrame;

    private Stopwatch StopWatch { get; }

    private double _lastElapsed;
    private double _elapsedSinceFrameStart;
    private int _currentIndex;
    private int _totalFrames;

    public Bitmap? Image => CurrentFrame?.TextureName is not null ? BitmapBank.Get(CurrentFrame.TextureName) : null;

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
        double elapsed = StopWatch.ElapsedMilliseconds - _lastElapsed;
        _elapsedSinceFrameStart += elapsed;

        if (CurrentFrame is not null && ActiveAnimation is not null)
        {
            if (_elapsedSinceFrameStart >= CurrentFrame.FrameLength)
            {
                var remainder = _elapsedSinceFrameStart - CurrentFrame.FrameLength;

                _currentIndex++;
                if (_currentIndex >= _totalFrames)
                {
                    _currentIndex = 0;
                }

                CurrentFrame = ActiveAnimation.Frames[_currentIndex];
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

        ActiveAnimation = newValue;
        Restart();

        void FramesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            _totalFrames = (sender as IList)?.Count ?? 0;
            Restart();
        }
    }

    private void Restart()
    {
        _totalFrames = ActiveAnimation?.Frames.Count ?? 0;
        CurrentFrame = ActiveAnimation?.Frames.FirstOrDefault();
        _currentIndex = 0;
        _lastElapsed = 0;
        _elapsedSinceFrameStart = 0;
        StopWatch.Restart();
    }

    void IRecipient<Messages.ActiveAnimationChanged>.Receive(Messages.ActiveAnimationChanged message)
    {
        ActiveAnimation = message.AnimationViewModel;
    }
}