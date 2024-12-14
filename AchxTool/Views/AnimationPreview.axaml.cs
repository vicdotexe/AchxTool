using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace AchxTool.Views;

public partial class AnimationPreview : UserControl
{
    public AnimationPreview()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is AnimationRunnerViewModel { IsRunning: true } vm)
        {
            vm.IsRunning = false;

            vm.PropertyChanged += (o, args) =>
            {
                if (args.PropertyName == nameof(vm.ActiveAnimation))
                {
                    Size[] sizes =
                        [.. vm.ActiveAnimation?.Frames.Select(f => new Size(f.Width, f.Height)) ?? [new(64, 64)]];

                    Matrix containmentMatrix = CreateZoomAndContain(ZoomBorder.Bounds.Size, sizes);

                    ZoomBorder.SetMatrix(containmentMatrix);
                }
            };
        }
    }

    public static Matrix CreateZoomAndContain(Size container, params Size[] contents)
    {

        (double mw, double mh) = contents.Aggregate((maxWidth: (double)0, maxHeight: (double)0),
            (acc, size) => (
                maxWidth: Math.Max(acc.maxWidth, size.Width),
                maxHeight: Math.Max(acc.maxHeight, size.Height)
            )
        );

        var z = Math.Min(container.Width / mw, container.Height / mh) * 0.9;

        return Matrix.CreateScale(z, z) * Matrix.CreateTranslation(container.Width / 2, container.Height / 2);
    }

    public void FrameSlider_Focused(object? sender, GotFocusEventArgs e)
    {
        if (DataContext is AnimationRunnerViewModel {IsRunning: true} vm)
        {
            vm.IsRunning = false;
        }
    }
}