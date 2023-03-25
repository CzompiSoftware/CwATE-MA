using Markdig.Helpers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System.Diagnostics;

namespace Markdig.Extensions.Xmd.Alert;

[DebuggerDisplay("#{" + nameof(AlertBlock) + "}")]
public class AlertBlock : LeafInline
{
    internal MarkdownPipelineBuilder _pipeline;
    private string content;

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
    public string Content
    {
        get
        {
            _pipeline = _pipeline.UseAdvancedExtensions().UseXmdLanguage();
            var pipeline = _pipeline.Build();
            var html = Markdown.ToHtml(content, pipeline);
            return html;
        }
        internal set => content = value;
    }

    public string Type { get; set; }
}
