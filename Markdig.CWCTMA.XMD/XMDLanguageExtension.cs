using Markdig.Renderers;
using ColorCode;
using Markdig.CWCTMA.XMD.CSCode;
using Markdig.CWCTMA.XMD.Alert;

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

            if (!pipeline.InlineParsers.Contains<AlertBlockParser>())
            {
                pipeline.InlineParsers.Add(new AlertBlockParser());
            }

            if (!pipeline.InlineParsers.Contains<CSCodeInlineParser>())
            {
                pipeline.InlineParsers.Add(new CSCodeInlineParser());
            }

            if (!pipeline.InlineParsers.Contains<CSCodeBlockParser>())
            {
                pipeline.InlineParsers.Add(new CSCodeBlockParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {

            if (!renderer.ObjectRenderers.Contains<AlertBlockRenderer>())
            {
                renderer.ObjectRenderers.Add(new AlertBlockRenderer());
            }

            if (!renderer.ObjectRenderers.Contains<CSCodeInlineRenderer>())
            {
                renderer.ObjectRenderers.Add(new CSCodeInlineRenderer());
            }

            if (!renderer.ObjectRenderers.Contains<CSCodeBlockRenderer>())
            {
                renderer.ObjectRenderers.Add(new CSCodeBlockRenderer());
            }
        }

    }
}
