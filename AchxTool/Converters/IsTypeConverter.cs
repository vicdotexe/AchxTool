using System.Globalization;

using Avalonia.Data.Converters;

namespace AchxTool.Converters;

public class IsTypeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (parameter is Type targetTypeParameter)
        {
            return value?.GetType() == targetTypeParameter;
        }

        if (parameter is string typeName)
        {
            var targetTypeFromString = Type.GetType(typeName);
            return targetTypeFromString != null && value?.GetType() == targetTypeFromString;
        }

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}