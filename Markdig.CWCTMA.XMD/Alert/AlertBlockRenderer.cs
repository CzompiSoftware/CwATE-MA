using Markdig.CWCTMA.XMD.CSCode;
using Markdig.Renderers.Html;
using Markdig.Renderers;

namespace Markdig.CWCTMA.XMD.Alert
{
    public class AlertBlockRenderer : HtmlObjectRenderer<AlertBlock>
    {

        protected override void Write(HtmlRenderer renderer, AlertBlock obj)
        {
            renderer.Write($"<div class=\"alert alert-{obj.Type.ToLower()}\">{Markdown.ToHtml(obj.Content)}</div>");
        }
    }
}
