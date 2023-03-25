using Markdig.Helpers;

namespace Markdig.Extensions.Xmd;

public static class XmdLanguageExtensions
{
    public static MarkdownPipelineBuilder UseXmdLanguage(this MarkdownPipelineBuilder pipeline)
    {
        pipeline.Extensions.Add(new XmdLanguageExtension());
        return pipeline;
    }
    internal static void MoveForward(this StringSlice slice, int v)
    {
        for (int i = 0; i < v; i++)
        {
            slice.NextChar();
        }
    }

}
