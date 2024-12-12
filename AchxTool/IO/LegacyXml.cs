using System.Xml.Serialization;

namespace AchxTool.IO;

[XmlRoot("AnimationChainArraySave")]
public class AnimationChainArraySave
{
    [XmlElement("FileRelativeTextures")]
    public bool FileRelativeTextures { get; set; }

    [XmlElement("TimeMeasurementUnit")]
    public string TimeMeasurementUnit { get; set; }

    [XmlElement("CoordinateType")]
    public string CoordinateType { get; set; }

    [XmlElement("AnimationChain")]
    public List<AnimationChain> AnimationChains { get; set; } = new();
}

public class AnimationChain
{
    [XmlElement("Name")]
    public string Name { get; set; }

    [XmlElement("Frame")]
    public List<Frame> Frames { get; set; } = new();
}

public class Frame
{
    [XmlElement("TextureName")]
    public string TextureName { get; set; }

    [XmlElement("FrameLength")]
    public double FrameLength { get; set; }

    [XmlElement("LeftCoordinate")]
    public int LeftCoordinate { get; set; }

    [XmlElement("RightCoordinate")]
    public int RightCoordinate { get; set; }

    [XmlElement("TopCoordinate")]
    public int TopCoordinate { get; set; }

    [XmlElement("BottomCoordinate")]
    public int BottomCoordinate { get; set; }
}