using Markdig.Xmd;
using System;

namespace Markdig.Xmd
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
