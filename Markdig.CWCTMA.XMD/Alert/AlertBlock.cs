using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.CWCTMA.XMD.Alert
{
    public class AlertBlock : LeafInline
    {
        public AlertBlock()
        {
        }

        /// <summary>
        /// The trimmed source code.
        /// </summary>
        public string Content { get; set; }
        public string Type { get; set; }
    }
}
