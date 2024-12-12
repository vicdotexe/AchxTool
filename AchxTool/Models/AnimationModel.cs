using AchxTool.ViewModels;
using AchxTool.ViewModels.Nodes;

namespace AchxTool.Models
{
    public record class AnimationModel(string? Name = default, List<FrameModel>? Frames = default);

    public record class FrameModel(
        string? TextureName = default,
        int Length = default,
        int Left = default,
        int Top = default,
        int Right = default,
        int Bottom = default,
        bool FlipHorizontal = default,
        bool FlipVertical = default,
        string? FrameName = default);

    public class Collider();
}