using CzSoft.CwateMa.Extensions;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Markdig.Extensions.Xmdl.Lua;
using Markdig;
using Markdig.Prism;
using CzSoft.CwateMa.Model;
using CzSoft.CwateMa.Model.Xmdl;
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
        "php"
    ];
    private readonly List<string> _supportedExtensions = [
        "xmdl", 
        // "xmd"
    ];

    private MarkdownParserContext _context = new();

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
                                Logger.LogDebug("File found: {actualFilename}", actualFilename);
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
                            Logger.LogDebug("File found: {actualFilename}", actualFilename);
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
                            Logger.LogDebug("File found: {actualFilename}", actualFilename);
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
            Logger.LogDebug("File not found: {basePath}", basePath);
            return null;
        }
    }

    private string TryGetActualFileName(string filename)
    {
        try
        {
            var actualFilename = filename.GetActualFileName();
            Logger.LogDebug("TryGetActualFileName(): {actualFilename}", actualFilename);
            return actualFilename;
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
    
    [Inject] protected IHttpContextAccessor HttpContextAccessor {get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder?.AddContent(0, ChildContent);
    }

    private async Task<string> LoadPageContent()
    {
        CurrentPage = Remaining;

        var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        _context.Properties.Remove("page");
        _context.Properties.Add("page", new CwatePageContext(url: NavigationManager.BaseUri, route: uri.AbsolutePath, query: uri.Query + uri.Fragment));
    
        if (File.Exists(FileName))
        {
            var cdnUri = new Uri(Globals.Config.CdnUrl);
            var markdownFile = await File.ReadAllTextAsync(FileName);
            var header = $"{markdownFile[..(markdownFile.IndexOf("</metadata>", StringComparison.OrdinalIgnoreCase) + "</metadata>".Length)]}";
            var meta = header.ParseXml<Metadata>();
            _title = meta.Title;
            markdownFile = markdownFile[header.Length..];

            markdownFile = Enum.GetNames<AssetType>().Aggregate(markdownFile, (current, t) => current.Replace($"${{assetTypeRoot:{t}}}", $"${{cdnRoot}}{t.ToLowerInvariant()}/", StringComparison.OrdinalIgnoreCase));
            markdownFile = markdownFile.Replace("${{cdnRoot}}", "${cdnScheme}://${cdnHost}/", StringComparison.OrdinalIgnoreCase);
            markdownFile = markdownFile.Replace("${{cdnScheme}}", $"{cdnUri.Scheme}", StringComparison.OrdinalIgnoreCase);
            markdownFile = markdownFile.Replace("${{cdnHost}}", $"{cdnUri.DnsSafeHost}", StringComparison.OrdinalIgnoreCase);
            var content = Markdown.ToHtml(markdownFile, _markdownPipeline, _context);

            if (!meta.ShowModifiedAt) return content;
            
            content += $"""<p style="text-align: right;">Created{(meta.ReleasedAt == meta.ModifiedAt ? " and last modified" : "")} at <b>{meta.ReleasedAt.ToSqlTimeString().Replace("T", " ")}</b></p>""";

            if (meta.ReleasedAt == meta.ModifiedAt) return content;
            
            content += "\r\n" + $"""<p style="text-align: right;">Last modified at <b>{meta.ModifiedAt.ToSqlTimeString().Replace("T", " ")}</b></p>""";

            return content;
        }
        else
        {
            _title = "Not found";
            return Markdown.ToHtml("]>danger< # Error 404\r\n] Sorry, there's nothing at this address.", _markdownPipeline, _context);
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await Task.Run(() => _markdownPipeline ??= new MarkdownPipelineBuilder().UseAdvancedExtensions().UsePrism().UseXmdlLua(_options, new(NavigationManager.Uri), false).UseCdnForImages().Build());
        //content = await LoadPageContent();
        await base.OnInitializedAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        Remaining ??= "index.html";
        _markdownPipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UsePrism().UseXmdlLua(_options, new(NavigationManager.Uri), false).UseCdnForImages().Build();

        if (CurrentPage != Remaining)
        {
            _title = "Loading...";
            _content = null;
            StateHasChanged();
            _content = await LoadPageContent();
            StateHasChanged();
        }
        await base.OnParametersSetAsync();
    }

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