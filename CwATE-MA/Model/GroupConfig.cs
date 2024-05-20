using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cwatema.Model;

public class GroupConfig
{
    public string Current { get; set; }
    public List<GroupConfigItem> Groups { get; set; }
}
