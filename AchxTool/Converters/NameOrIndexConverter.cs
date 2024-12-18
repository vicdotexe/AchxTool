using System.Collections;

using Avalonia.Data.Converters;
using System.Globalization;

using AchxTool.ViewModels;
using AchxTool.ViewModels.Nodes;

using AchxNodeViewModel = AchxTool.ViewModels.Nodes.AchxNodeViewModel;
using FrameViewModel = AchxTool.ViewModels.Nodes.FrameViewModel;

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
                AnimationViewModel => topLevelNodes.IndexOf(node),
                FrameViewModel frame => FindFrameIndex(frame),
                _ => -1
            };
        }

        return parameter is string title && !string.IsNullOrWhiteSpace(title) ? $"{title} - {index}" : $"{index}";

        int FindFrameIndex(FrameViewModel frame)
        {
            foreach (var chain in topLevelNodes.OfType<AnimationViewModel>())
            {
                if (chain.Frames.IndexOf(frame) is var i && i != -1)
                    return i;
            }
            return -1;
        }
    }

}