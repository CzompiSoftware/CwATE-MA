using Markdig.Xmd.CSCode;
using Markdig.Renderers.Html;
using Markdig.Renderers;

namespace Markdig.Xmd.Alert
{
    public class AlertBlockRenderer : HtmlObjectRenderer<AlertBlock>
    {
        private MarkdownPipeline _pipeline;

        public AlertBlockRenderer(): base()
        {
        }
        
        public AlertBlockRenderer(MarkdownPipeline pipeline) : base()
        {
            _pipeline = pipeline;
        }

        protected override void Write(HtmlRenderer renderer, AlertBlock obj)
        {
            renderer.Write($"<div class=\"alert alert-{obj.Type.ToLower()}\">{Markdown.ToHtml(obj.Content, _pipeline)}</div>");
        }
    }
}
