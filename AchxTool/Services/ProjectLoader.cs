using System.Xml.Serialization;
using AchxTool.IO;
using AchxTool.ViewModels;
using AchxTool.ViewModels.Nodes;

using CommunityToolkit.Mvvm.Messaging;

using FrameViewModel = AchxTool.ViewModels.Nodes.FrameViewModel;

namespace AchxTool.Services
{
    public interface IProjectLoader
    {
        ProjectViewModel CurrentProject { get; set; }
        Task<ProjectViewModel?> LoadProjectAsync(string path);
    }

    public class ProjectLoader : IProjectLoader
    {
        private ProjectViewModel _currentProject;

        public ProjectViewModel CurrentProject
        {
            get => _currentProject;
            set
            {
                if (value != _currentProject)
                {
                    _currentProject = value;
                    Messenger.Send<ProjectLoadedMessage>(new(value));
                }
            }
        }

        private IViewModelFactory Factory { get; }
        private IMessenger Messenger { get; }

        public async Task<ProjectViewModel?> LoadProjectAsync(string path)
        {
            if (new FileInfo(path) is not { Exists: true } fileInfo)
            {
                return null;
            }

            if (await IsXml(fileInfo))
            {
                return await LoadLegacyXml(fileInfo);
            }

            return null;
        }

        public ProjectLoader(IViewModelFactory factory, IMessenger messenger)
        {
            Factory = factory;
            _currentProject = factory.New<ProjectViewModel>();
            Messenger = messenger;
        }

        private static async Task<bool> IsXml(FileInfo file)
        {
            await using FileStream fileStream = file.OpenRead();
            using StreamReader streamReader = new StreamReader(fileStream);

            string? firstLine = await streamReader.ReadLineAsync();

            return firstLine?.Trim().StartsWith("<?xml") ?? false;
        }

        private async Task<ProjectViewModel?> LoadLegacyXml(FileInfo fileInfo)
        {
            await using FileStream fileStream = fileInfo.OpenRead();
            using StreamReader streamReader = new (fileStream);

            XmlSerializer serializer = new (typeof(AnimationChainArraySave));

            if (serializer.Deserialize(fileStream) is not AnimationChainArraySave result)
            {
                return null;
            }

            ProjectViewModel project = new()
            {
                ProjectType = ProjectType.Legacy,
                Version = -1,
                FilePath = fileInfo.FullName,
                TexturePathsAreRelative = result.FileRelativeTextures
            };

            foreach (AnimationChain chain in result.AnimationChains)
            {
                AnimationViewModel animation = Factory.New<AnimationViewModel>(a =>
                {
                    a.Name = chain.Name;
                    
                    foreach (Frame frame in chain.Frames)
                    {
                        FrameViewModel frameViewModel = Factory.New<FrameViewModel>(f =>
                        {
                            f.TextureName = frame.TextureName;
                            f.FrameLength = frame.FrameLength;
                            f.X = frame.LeftCoordinate;
                            f.Y = frame.TopCoordinate;
                            f.Width = frame.RightCoordinate - frame.LeftCoordinate;
                            f.Height = frame.BottomCoordinate - frame.TopCoordinate;
                        });
                        a.Frames.Add(frameViewModel);
                    }
                });
                
                project.Animations.Add(animation);
            }
            return project;
        }
    }
}