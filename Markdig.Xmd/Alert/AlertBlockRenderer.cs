using Markdig.Renderers.Html;
using Markdig.Renderers;
using Markdig.Extensions.Xmd;

namespace Markdig.Extensions.Xmd.Alert;

public class AlertBlockRenderer : HtmlObjectRenderer<AlertBlock>
{
    private MarkdownPipeline _pipeline;

    public AlertBlockRenderer() : base()
    {
    }

    public AlertBlockRenderer(MarkdownPipeline pipeline) : base()
    {
        _pipeline = pipeline;
    }

    protected override void Write(HtmlRenderer renderer, AlertBlock obj)
    {
        var pipeline = new MarkdownPipelineBuilder();
        pipeline = pipeline.UseAdvancedExtensions().UseXmdLanguage();
        renderer.Write("<div class=\"alert alert-").Write(obj.Type).Write("\">").Write(obj.Content).Write("</div>");
    }
}
