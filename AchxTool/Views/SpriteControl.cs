using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace AchxTool.Views;
public class SpriteControl : Control
{
    public static readonly StyledProperty<Bitmap?> ImageProperty =
        AvaloniaProperty.Register<SpriteControl, Bitmap?>(nameof(Image));

    public static readonly StyledProperty<int> XProperty =
        AvaloniaProperty.Register<SpriteControl, int>(nameof(X));

    public static readonly StyledProperty<int> YProperty =
        AvaloniaProperty.Register<SpriteControl, int>(nameof(Y));    
    
    public static readonly StyledProperty<bool> FlipHorizontalProperty =
        AvaloniaProperty.Register<SpriteControl, bool>(nameof(FlipHorizontal));

    public static readonly StyledProperty<bool> FlipVerticalProperty =
        AvaloniaProperty.Register<SpriteControl, bool>(nameof(Y));


    public Bitmap? Image
    {
        get => GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public int X
    {
        get => GetValue(XProperty);
        set => SetValue(XProperty, value);
    }

    public int Y
    {
        get => GetValue(YProperty);
        set => SetValue(YProperty, value);
    }

    public bool FlipHorizontal
    {
        get => GetValue(FlipHorizontalProperty);
        set => SetValue(FlipHorizontalProperty, value);
    }

    public bool FlipVertical
    {
        get => GetValue(FlipVerticalProperty);
        set => SetValue(FlipVerticalProperty, value);
    }


    public override void Render(DrawingContext context)
    {
        if (Image != null)
        {
            var sourceRect = new Rect(X, Y, Width, Height);
            var destinationRect = new Rect(0, 0, Width, Height);

            Matrix scale = Matrix.CreateScale(FlipHorizontal ? -1 : 1, FlipVertical ? -1 : 1);
            Matrix translate = Matrix.CreateTranslation(FlipHorizontal ? Width : 0, FlipVertical ? Height : 0);
            using (context.PushTransform(scale * translate))
            {

                context.DrawImage(Image, sourceRect, destinationRect);
            }
        }
    }

    public SpriteControl()
    {
        AffectsRender<SpriteControl>(XProperty, YProperty, HeightProperty, WidthProperty, ImageProperty, FlipHorizontalProperty, FlipVerticalProperty);
        RenderOptions.SetBitmapInterpolationMode(this, BitmapInterpolationMode.None);
    }
}