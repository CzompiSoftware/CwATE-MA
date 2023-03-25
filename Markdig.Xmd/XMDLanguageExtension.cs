using Markdig.Renderers;
using Markdig.Extensions.Xmd.CSCode;
using Markdig.Extensions.Xmd.Alert;

namespace Markdig.Extensions.Xmd;

public class XmdLanguageExtension : IMarkdownExtension
{
    public XmdLanguageExtension()
    {
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        pipeline.InlineParsers.ReplaceOrAdd<CSCodeInlineParser>(new CSCodeInlineParser());
        pipeline.InlineParsers.ReplaceOrAdd<CSCodeBlockParser>(new CSCodeBlockParser());
        pipeline.InlineParsers.ReplaceOrAdd<AlertBlockParser>(new AlertBlockParser(pipeline));
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        renderer.ObjectRenderers.ReplaceOrAdd<CSCodeInlineRenderer>(new CSCodeInlineRenderer());
        renderer.ObjectRenderers.ReplaceOrAdd<CSCodeBlockRenderer>(new CSCodeBlockRenderer());
        renderer.ObjectRenderers.ReplaceOrAdd<AlertBlockRenderer>(new AlertBlockRenderer(pipeline));
    }

}
