using System.Globalization;

using AchxTool.ViewModels;

using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace AchxTool.Converters
{
    public class FrameToRectConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is FrameViewModel frame)
            {
                return new RectangleGeometry(new Rect(frame.X, frame.Y, frame.Width, frame.Height));
            }

            return new RectangleGeometry();
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}