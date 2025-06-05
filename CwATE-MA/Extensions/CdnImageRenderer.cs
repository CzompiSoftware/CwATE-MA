using CzSoft.CwateMa.Model;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Renderers.Html.Inlines;
using Markdig.Syntax.Inlines;

namespace CzSoft.CwateMa.Extensions;

public class CdnImageRenderer : HtmlObjectRenderer<LinkInline>
{
    private readonly Config _config;

    public CdnImageRenderer(Config config)
    {
        _config = config;
    }

    protected override void Write(HtmlRenderer renderer, LinkInline link)
    {
        if (link.IsImage)
        {
            if (!string.IsNullOrEmpty(link.Url) &&
                !link.Url.StartsWith("http://") &&
                !link.Url.StartsWith("https://"))
            {
                link.Url = $"https://cdn.czsoft.hu/img/{_config.Id}@v1/{link.Url}";
            }
        }

        // Use the default rendering for the (possibly modified) link
        var defaultRenderer = new LinkInlineRenderer();
        defaultRenderer.Write(renderer, link);
    }
}