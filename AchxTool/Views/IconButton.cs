using Avalonia;
using Avalonia.Controls;

using Material.Icons;

namespace AchxTool.Views
{
    public class IconButton : Button
    {
        protected override Type StyleKeyOverride => typeof(Button);

        public static StyledProperty<MaterialIconKind> IconProperty =
            AvaloniaProperty.Register<IconButton, MaterialIconKind>(nameof(Icon));

        public static StyledProperty<int> IconSizeProperty =
            AvaloniaProperty.Register<IconButton, int>(nameof(IconSize), 16);

        public int IconSize
        {
            get => GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        public MaterialIconKind Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        static IconButton()
        {
            AffectsRender<IconButton>(IconProperty, IconSizeProperty);
        }
    }
}