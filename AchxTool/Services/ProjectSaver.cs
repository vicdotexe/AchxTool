using AchxTool.ViewModels;

namespace AchxTool.Services;

public interface IProjectSerializer
{
    string SerializeToLegacyXml(ProjectViewModel project);
    string Serialize(ProjectViewModel project);

}

public class ProjectSerializer : IProjectSerializer
{
    public ProjectSerializer(Lazy<INodeTree> nodeList)
    {
    }

    public string SerializeToLegacyXml(ProjectViewModel project)
    {
        throw new NotImplementedException();
    }
    public string Serialize(ProjectViewModel project)
    {
        throw new NotImplementedException();
    }
}