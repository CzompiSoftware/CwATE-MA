using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.Xmd.Alert
{
    public class AlertBlock : LeafInline
    {
        private MarkdownPipelineBuilder _pipeline;

        public AlertBlock()
        {
        }

        public AlertBlock(MarkdownPipelineBuilder pipeline)
        {
            _pipeline = pipeline;
        }

        /// <summary>
        /// The trimmed source code.
        /// </summary>
        public string Content { get; set; }
        public string Type { get; set; }
    }
}
