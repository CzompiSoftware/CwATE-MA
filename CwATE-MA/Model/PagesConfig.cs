﻿using Cwatema.Model.Xmd;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Cwatema.Model;

[Serializable]
[DesignerCategory("code")]
[XmlType(AnonymousType = true)]
[XmlRoot(ElementName = "Pages",Namespace = "", IsNullable = false)]
public class PagesConfig
{
    [XmlElement(ElementName ="Page")]
    public List<Metadata> Page {  get; set; }
}