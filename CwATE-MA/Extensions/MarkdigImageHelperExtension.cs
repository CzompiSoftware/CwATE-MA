using CzSoft.CwateMa.Model;
using Markdig;
using Markdig.Renderers;
using Markdig.Syntax;

namespace CzSoft.CwateMa.Extensions;

public class MarkdigImageHelperExtension : IMarkdownExtension
{
    private readonly Config _config;

    public MarkdigImageHelperExtension()
    {
        _config = Globals.Config;
    }

    public void Setup(MarkdownPipelineBuilder pipeline)
    {
        // No setup required before parsing
    }

    public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
    {
        renderer.ObjectRenderers.ReplaceOrAdd<CdnImageRenderer>(new CdnImageRenderer(_config));
    }
}