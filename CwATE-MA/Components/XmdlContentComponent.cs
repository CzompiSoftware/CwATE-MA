using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Markdig.Extensions.Xmdl.Lua;
using Markdig;
using Markdig.Prism;
using CzSoft.CwateMa.Model;
using CzSoft.CwateMa.Model.Xmd;
using CzSoft.CwateMa.Helpers;
using Markdig.Extensions.Xmdl;

namespace CzSoft.CwateMa.Components;

public class XmdlContentComponent : ComponentBase, IAsyncDisposable
{
    protected IJSObjectReference _module;
    private readonly ExecutableCodeOptions _options = new()
    {
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
    private readonly List<string> _supportedVirtualExtensions = [
        "html", 
        "htm"
    ];
    private readonly List<string> _supportedExtensions = [
        "xmdl", 
        // "xmd"
    ];

    private string FileName
    {
        get
        {
            var baseFilename = Remaining ?? "index";
            var basePath = Path.Combine(Globals.ContentDirectory, baseFilename);

            if (Path.HasExtension(baseFilename))
            {
                foreach (var virtualExtension in _supportedVirtualExtensions)
                {
                    foreach (var supportedExtension in _supportedExtensions)
                    {
                        try
                        {
                            var actualFilename = TryGetActualFileName(basePath.Replace(virtualExtension, supportedExtension));
                            if (actualFilename != null)
                            {
                                return actualFilename;
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogDebug(e.Message);
                            //throw;
                        }
                    }
                }
            }
            else
            {
                // Remaining does not have an extension, so try appending supported extensions
                foreach (string extension in _supportedExtensions)
                {
                    try
                    {
                        string candidate = basePath + "." + extension;
                        string actualFilename = TryGetActualFileName(candidate);
                        if (actualFilename != null)
                        {
                            return actualFilename;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogDebug(e.Message);
                    }
                }

                // Also try as a directory with index files
                string directoryPath = Path.Combine(Globals.ContentDirectory, baseFilename);
                foreach (string extension in _supportedExtensions)
                {
                    try
                    {
                        string candidate = Path.Combine(directoryPath, "index." + extension);
                        string actualFilename = TryGetActualFileName(candidate);
                        if (actualFilename != null)
                        {
                            return actualFilename;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogDebug(e.Message);
                    }
                }
            }

            Logger.LogWarning("File not found: {Remaining}", Remaining);
            return null;
        }
    }

    private string TryGetActualFileName(string filename)
    {
        try
        {
            return filename.GetActualFileName();
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    [Parameter] public string Remaining { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }

    public string CurrentPage { get; protected set; }

    
    [Inject] protected ILogger<XmdlContentComponent> Logger { get; set; }

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
        await Task.Run(() => _markdownPipeline ??= new MarkdownPipelineBuilder().UseAdvancedExtensions().UsePrism().UseXmdlLua(_options, new(NavigationManager.Uri)).Build());
        //content = await LoadPageContent();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        Remaining ??= "index.xmdl";
        _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UsePrism().UseXmdlLua(_options, new(NavigationManager.Uri)).Build();

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