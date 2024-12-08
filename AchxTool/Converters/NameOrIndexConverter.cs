using System.Collections;

using Avalonia.Data.Converters;
using System.Globalization;

using AchxTool.ViewModels;

namespace AchxTool.Converters;
public class NameOrIndexConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {

        if (values[0] is string name && !string.IsNullOrWhiteSpace(name))
        {
            return name;
        }

        int index = -1;

        if (values is
            [
                ..,
                AchxNodeViewModel node,
                IList topLevelNodes
            ])
        {
            index = node switch
            {
                AnimationChainViewModel => topLevelNodes.IndexOf(node),
                AnimationFrameViewModel frame => FindFrameIndex(frame),
                _ => -1
            };
        }

        return parameter is string title && !string.IsNullOrWhiteSpace(title) ? $"{title} - {index}" : $"{index}";

        int FindFrameIndex(AnimationFrameViewModel frame)
        {
            foreach (var chain in topLevelNodes.OfType<AnimationChainViewModel>())
            {
                if (chain.Frames.IndexOf(frame) is var i && i != -1)
                    return i;
            }
            return -1;
        }
    }

}