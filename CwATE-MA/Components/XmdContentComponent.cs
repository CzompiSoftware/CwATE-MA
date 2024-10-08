using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Markdig.Extensions.Xmd.CSCode;
using Markdig;
using Markdig.Prism;
using Markdig.Extensions.Xmd;
using CzSoft.CwateMa.Model;
using CzSoft.CwateMa.Model.Xmd;
using CzSoft.CwateMa.Helpers;

namespace CzSoft.CwateMa.Components;

public class XmdContentComponent : ComponentBase, IAsyncDisposable
{
    protected IJSObjectReference _module;
    private readonly CSCodeOptions _options = new()
    {
        OptimizationLevel = Microsoft.CodeAnalysis.OptimizationLevel.Release,
#if RELEASE
        MinimumLogLevel = LogLevel.Warning,
#else
        MinimumLogLevel = LogLevel.Trace,
#endif
        WorkingDirectory = Globals.ContentDirectory
    };
    protected MarkdownPipeline _markdownPipeline;
    protected string _title; // = "Loading...";
    protected string _content;

    protected string FileName
    {
        //TODO: Sort out this mess (2024-07-11)
        get
        {
            var filename = Path.Combine(Globals.ContentDirectory, $"{Remaining ?? "index.xmdl"}");
            try
            {
                filename = filename.GetActualFileName();
            }
            catch (FileNotFoundException)
            {
                FileInfo fif = new(filename);
                if(fif.Extension == "." && !fif.Exists)
                {
                    filename = Path.Combine(Globals.ContentDirectory, $"{Remaining.TrimEnd('/')}/index.xmdl");
                    try
                    {
                        filename = filename.GetActualFileName();
                    }
                    catch (FileNotFoundException)
                    {
                        filename = Path.Combine(Globals.ContentDirectory, $"{Remaining.TrimEnd('/')}/index.xmd");
                        try
                        {
                            filename = filename.GetActualFileName();
                        }
                        catch (FileNotFoundException fex)
                        {
                            Logger.LogWarning(fex, "No file!");
                        }
                    }
                }
            }
            return filename;
        }
    }

    [Parameter] public string Remaining { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    public string CurrentPage { get; set; }

    
    [Inject] protected ILogger<XmdContentComponent> Logger { get; set; }

    [Inject] protected NavigationManager NavigationManager { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder?.AddContent(0, ChildContent);
    }

    private async Task<string> LoadPageContent()
    {
        CurrentPage = Remaining;
        if (File.Exists(FileName))
        {
            var cdnUri = new Uri(Globals.Config.CdnUrl);
            var markdownFile = await File.ReadAllTextAsync(FileName);
            var header = $"{markdownFile[..(markdownFile.IndexOf("</metadata>", StringComparison.OrdinalIgnoreCase) + "</metadata>".Length)]}";
            var meta = header.ParseXml<Metadata>();
            _title = meta.Title;
            markdownFile = markdownFile[header.Length..];

            foreach (var t in Enum.GetNames<AssetType>())
            {
                markdownFile = markdownFile.Replace($"${{assetTypeRoot:{t}}}", $"${{cdnRoot}}{t.ToLowerInvariant()}/", StringComparison.OrdinalIgnoreCase);
            }
            markdownFile = markdownFile.Replace($"${{cdnRoot}}", "https://${cdnHost}/", StringComparison.OrdinalIgnoreCase);
            markdownFile = markdownFile.Replace($"${{cdnScheme}}", $"{cdnUri.Scheme}", StringComparison.OrdinalIgnoreCase);
            markdownFile = markdownFile.Replace($"${{cdnHost}}", $"{cdnUri.DnsSafeHost}", StringComparison.OrdinalIgnoreCase);
            var content = Markdown.ToHtml(markdownFile, _markdownPipeline);

            if (meta.ShowModifiedAt)
            {
                content += $"""<p style="text-align: right;">Created{(meta.ReleasedAt == meta.ModifiedAt ? " and last modified" : "")} at <b>{meta.ReleasedAt.ToSqlTimeString().Replace("T", " ")}</b></p>""";
                if (meta.ReleasedAt != meta.ModifiedAt)
                {
                    content += "\r\n" + $"""<p style="text-align: right;">Last modified at <b>{meta.ModifiedAt.ToSqlTimeString().Replace("T", " ")}</b></p>""";
                }
            }

            return content;
        }
        else
        {
            _title = "Not found";
            return Markdown.ToHtml("]>danger< # Error 404\r\n] Sorry, there's nothing at this address.", _markdownPipeline);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Run(() => _markdownPipeline ??= new MarkdownPipelineBuilder().UseAdvancedExtensions().UsePrism().UseXmdLanguage(_options, new(NavigationManager.Uri)).Build());
        //content = await LoadPageContent();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        Remaining ??= "index";
        _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UsePrism().UseXmdlLanguage(_options, new(NavigationManager.Uri)).Build();

        if (CurrentPage != Remaining)
        {
            //await JsRuntime.InvokeVoidAsync("removeElementsByClass", "code-toolbar"); // Fixes issue caused by PrismJS
            _title = "Loading...";
            _content = null;
            StateHasChanged();
            _content = await LoadPageContent();
            StateHasChanged();
        }
        await base.OnParametersSetAsync();
    }

    //protected override async Task OnAfterRenderAsync(bool firstRender)
    //{
    //    if (firstRender)
    //    {
    //        _module = await JSRuntimeExtensions.InvokeAsync<IJSObjectReference>(JsRuntime, "import", new object[1] { "./_content/MathJaxBlazor/mathJaxBlazor.js" });
    //    }

    //    if(_module != null)
    //    {
    //        await _module.InvokeVoidAsync("typesetPromise");
    //    }
    //    await JsRuntime.InvokeVoidAsync("Prism.highlightAll");
    //    await base.OnAfterRenderAsync(firstRender);
    //}

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            await _module.InvokeVoidAsync("typesetClear");
            await _module!.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }
}