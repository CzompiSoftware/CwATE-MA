
using System;
using System.ComponentModel;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Cwatema.Model.Xmd;

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
    public string Lang { get; set; } = "en";

    /// <remarks/>
    [XmlAttribute]
    public bool IsNavMenuItem { get; set; } = false;

    [XmlAttribute]
    public short NavMenuId { get; set; } = -1;

    /// <remarks/>
    [XmlAttribute]
    public bool ShowModifiedAt { get; set; } = false;
    
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
    public ImageData Image { get; set; } = new();

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Qualified)]
    public DateTime ReleasedAt { get; set; } = DateTime.Now;

    /// <remarks/>
    [XmlElement(Form = XmlSchemaForm.Qualified)]
    public DateTime ModifiedAt { get; set; } = DateTime.Now;

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
