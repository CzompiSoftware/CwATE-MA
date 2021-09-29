using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.CWCTMA.XMD.CSCode
{

    /// <summary>
    /// A C# source code inline element.
    /// </summary>
    /// <seealso cref="FencedCodeBlock" />
    public class CSCodeBlock : LeafInline
    {
        public CSCodeBlock()
        {
        }

        /// <summary>
        /// The trimmed source code.
        /// </summary>
        public string SourceCode { get; set; }
    }
}
