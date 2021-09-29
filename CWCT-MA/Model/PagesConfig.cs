using CWCTMA.Model.XMD;
using System.ComponentModel;
using System.Xml.Serialization;

namespace CWCTMA.Model
{
    [Serializable]
    [DesignerCategory("code")]
    [XmlType(AnonymousType = true)]
    [XmlRoot(ElementName = "Pages",Namespace = "", IsNullable = false)]
    public class PagesConfig
    {
        [XmlElement(ElementName ="Page")]
        public List<Metadata> Page {  get; set; }
    }
}