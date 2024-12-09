using Avalonia.Data.Converters;
using Avalonia;
using System.Globalization;

namespace AchxTool.Converters;

public class ScaleInverterConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is double scale)
        {
            return scale > 1 ? 1 / scale : scale;
        }

        return AvaloniaProperty.UnsetValue; // Return UnsetValue if the input is invalid
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Optional: Implement if two-way binding is required
        return value;
    }
}