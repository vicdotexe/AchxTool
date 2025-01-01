using System.Globalization;

using AchxTool.Services;
using AchxTool.ViewModels;

using Avalonia.Data.Converters;

namespace AchxTool.Converters
{
    public class TextureToFileInfoConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var textureManager = Locator.GetRequired<ITextureManager>();

            return value switch
            {
                FileInfo fileInfo => textureManager.Get(fileInfo),
                TextureViewModel textureViewModel => textureViewModel.FileInfo,
                _ => throw new InvalidOperationException()
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var textureManager = Locator.GetRequired<ITextureManager>();
            return value switch
            {
                FileInfo fileInfo => textureManager.Get(fileInfo),
                TextureViewModel textureViewModel => textureViewModel.FileInfo,
                _ => throw new InvalidOperationException()
            };
        }
    }
}