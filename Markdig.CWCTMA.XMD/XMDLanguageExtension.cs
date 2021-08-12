using Markdig.Renderers;
using ColorCode;

namespace Markdig.CWCTMA.XMD
{
    public class XMDLanguageExtension : IMarkdownExtension
    {
        private readonly IStyleSheet _customCss;

        public XMDLanguageExtension(IStyleSheet customCss = null)
        {
            _customCss = customCss;
        }

        public void Setup(MarkdownPipelineBuilder pipeline) 
        {

            if (!pipeline.InlineParsers.Contains<CSCodeInlineParser>())
            {
                // Insert the parser before any other parsers and use '#' as the character identifier
                pipeline.InlineParsers.Insert(0, new CSCodeInlineParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            HtmlRenderer htmlRenderer;
            ObjectRendererCollection renderers;

            htmlRenderer = renderer as HtmlRenderer;
            renderers = htmlRenderer?.ObjectRenderers;

            if (renderers != null && !renderers.Contains<CSCodeInlineRenderer>())
            {
                renderers.Insert(0, new CSCodeInlineRenderer());
            }
        }

    }
}
