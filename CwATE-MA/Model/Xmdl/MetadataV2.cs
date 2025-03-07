using System.ComponentModel;
using System.Xml.Serialization;

namespace CzSoft.CwateMa.Model.Xmdl;

public partial class Metadata
{
    
    /// <remarks/>
    public Navbar Navbar { get; set; } = new();
}

// <Navbar IsMember="true" Route="[]"/>
// <Navbar IsMember="true" Route="[Category name]->[]"/>
// <Navbar IsMember="true" Route="[Category name]->[Subcategory name]->[]"/>
[Serializable()]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
public partial class Navbar
{
    [XmlAttribute]
    public bool IsMember { get; set; } = false;
    
    [XmlAttribute]
    public string Route { get; set; } = "[]";
}
