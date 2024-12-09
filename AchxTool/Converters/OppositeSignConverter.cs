using Avalonia.Data.Converters;

namespace AchxTool.Converters;

public class OppositeSignConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {

        return value switch
        {
            int intNumber => -intNumber,
            double doubleNumber => -doubleNumber,
            float floatNumber => -floatNumber,
            _ => 0
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
    {
        // Reverse the conversion if necessary (optional)
        return Convert(value, targetType, parameter, culture);
    }
}