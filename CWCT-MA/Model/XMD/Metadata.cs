
using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CWCTMA.Model.XMD;
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(Namespace = "", IsNullable = false)]
public partial class Metadata
{
    /// <remarks/>
    [XmlAttribute]
    public string Type { get; set; }

    /// <remarks/>
    [XmlAttribute]
    public string Lang { get; set; }

    /// <remarks/>
    [XmlAttribute]
    public bool IsNavMenuItem { get; set; }

    /// <remarks/>
    [XmlAttribute]
    public bool ShowModifiedAt { get; set; }
    
    /// <remarks/>
    public string Id { get; set; }

    /// <remarks/>
    public string Title { get; set; }

    /// <remarks/>
    public object Keywords { get; set; }

    /// <remarks/>
    public string Description { get; set; }

    /// <remarks/>
    public string Search { get; set; }

    /// <remarks/>
    public ImageData Image { get; set; }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Qualified)]
    public DateTime ReleasedAt { get; set; }

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Qualified)]
    public DateTime ModifiedAt { get; set; }

}

/// <remarks/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
public partial class ImageData
{
    /// <remarks/>
    [XmlAttribute]
    public string Alt { get; set; }

    [XmlText]
    public string Value { get; set; }

}
