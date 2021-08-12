using System;

namespace Markdig.CWCTMA.XMD
{
    public static class XMDLanguageExtensions
    {
            public static MarkdownPipelineBuilder UseXMDLanguage(this MarkdownPipelineBuilder pipeline)
            {
                pipeline.Extensions.Add(new XMDLanguageExtension());
                return pipeline;
            }
        }
}
