using Markdig;

namespace CzSoft.CwateMa.Extensions;

public static class MarkdigImageHelperExtensions
{
    public static MarkdownPipelineBuilder UseCdnForImages(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.Add(new MarkdigImageHelperExtension());
        return pipeline;
    }
}