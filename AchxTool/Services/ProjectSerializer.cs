﻿using AchxTool.ViewModels;

namespace AchxTool.Services;

public interface IProjectSerializer
{
    string SerializeToLegacyXml(ProjectViewModel project);
    string Serialize(ProjectViewModel project);

}

public class ProjectSerializer : IProjectSerializer
{
    private INodeTree NodeTree { get; }

    public ProjectSerializer(INodeTree nodeTree)
    {
        NodeTree = nodeTree;
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