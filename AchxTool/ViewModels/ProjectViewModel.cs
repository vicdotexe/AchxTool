using AchxTool.Models;
using AchxTool.ViewModels.Nodes;

using CommunityToolkit.Mvvm.ComponentModel;

namespace AchxTool.ViewModels
{
    public enum ProjectType
    {
        Legacy,
        Modern
    }

    public class ProjectViewModel : ObservableObject
    {
        public ProjectType ProjectType { get; set; }
        public int Version { get; set; }

        public string? FilePath { get; set; }

        public List<AnimationViewModel> Animations { get; set; } = [];

        public bool TexturePathsAreRelative { get; set; }

    }
}