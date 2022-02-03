using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace Markdig.Xmd.CSCode
{
    public class CSCodeBlockRenderer : HtmlObjectRenderer<CSCodeBlock>
    {

        protected override void Write(HtmlRenderer renderer, CSCodeBlock obj)
        {
            renderer.Write($"{CodeHelper.ExecuteBlock(obj.SourceCode)}");
        }
    }
}