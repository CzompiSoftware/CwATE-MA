using CzSoft.CwateMa.Model;
using CzSoft.CwateMa.Helpers;
using CzSoft.CwateMa.Model.Xmdl;
using Markdig;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Runtime.Intrinsics.Arm;
using System.Text.Json;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using CzomPack.Cryptography;

namespace CzSoft.CwateMa.Model;

internal abstract class Globals
{
    #region Properties
    public static string CurrentPage { get; set; }

    public static ApplicationMetadata AppMeta { get; internal set; }
    public static string AppVersionString { get; internal set; }

    public static JsonSerializerOptions JsonSerializerOptions => new()
    {
        AllowTrailingCommas = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true
    };
    #endregion

    #region Directories
    public static string DataDirectory
    {
        get
        {
            var dir = Path.GetFullPath(Path.Combine("..", "data"));
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }
    }
    public static string ContentDirectory
    {
        get
        {
            var dir = Path.GetFullPath(Path.Combine(DataDirectory, "content"));
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }
    }
    public static string LogsDirectory
    {
        get
        {
            var dir = Path.GetFullPath(Path.Combine(DataDirectory, "logs"));
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }
    }
    #endregion

    #region Configs
    public static string ConfigFile => Path.Combine(DataDirectory, "config.json");
    public static Config Config { get; internal set; }

    public static string GroupFile => Path.Combine(DataDirectory, "group.json");
    public static GroupConfig Group { get; internal set; }

    //public static string PagesFile => Path.Combine(ContentDirectory, "pages.xml");
    public static Dictionary<string, Metadata> Pages { get; } = [];
    #endregion

    #region Methods
    //public static Metadata GetMetadata(string pageId) => Pages?.Page.FirstOrDefault(x => x.Id.Equals(pageId, StringComparison.OrdinalIgnoreCase)) ?? new() { };

    internal static string PrettifyHtml(string content)
    {
        HtmlParserOptions options = new()
        {
            // SkipScriptText = true,
            // SkipPlaintext = true,
            // // SkipDataText = true,
            // SkipRawText = true,
            // SkipCDATA = true,
            // SkipComments = true,
        };
        var parser = new HtmlParser(options);
        var document = parser.ParseDocument(content);
 
        var sw = new StringWriter();
        var formatter = new PrettyMarkupFormatter();
        document.ToHtml(sw, formatter);
        return sw.ToString();
    }

    internal static string MinifyHtml(string content)
    {
        var parser = new AngleSharp.Html.Parser.HtmlParser();
        var document = parser.ParseDocument(content);
 
        var sw = new StringWriter();
        document.ToHtml(sw, new AngleSharp.Html.MinifyMarkupFormatter());
        return sw.ToString();
    }

    internal static void LoadConfigs()
    {
        AppMeta = ApplicationMetadata.Parse(new GeneratedMetadata(), Dns.GetHostName());
        Console.WriteLine(JsonSerializer.Serialize(AppMeta));
        AppVersionString = AppMeta.Version.ToString(3);
        Console.WriteLine(AppVersionString);
        RefreshConfig();
        RefreshGroup();
        RefreshPages();
    }

    internal static void RefreshConfig()
    {
        if (!File.Exists(ConfigFile))
        {
            var defaultAppId = AppMeta.Name.Split("/")[0] + "DEV";
            defaultAppId = defaultAppId.ToLower();
            var defaultConfig = new Config
            {
                Id = defaultAppId,
                ShortName = AppMeta.Name + " DEV",
                FullName = AppMeta.FullName + " - Development Environment",

#if RELEASE
                CdnUrl = "https://cdn.czsoft.hu/",
#else
                CdnUrl = "https://cdn.czsoft.dev/",
#endif
                SiteURL = "/",
                GlobalUrl = "https://czompigroup.hu/",
                Meta = new()
                {
                    Title = AppMeta.Name + " DEV",
                    Description = AppMeta.FullName + " - Development Environment",
                    Image = null,
                    PrimaryColor = "#EAEAEA"
                },
                Themes = [
                    Theme.Parse("cwate", Version.Parse("2.0.0")),
                    Theme.Parse("cwate", Version.Parse("2.0.0"), $"style.{defaultAppId}.css"),
                    Theme.Parse("fluent-icons", Version.Parse("1.0.0"), "all.css"),
                    Theme.Parse("prism", Version.Parse("1.29.0"), "prism.css"),
                    Theme.Parse("prism", Version.Parse("1.29.0"), "prism.light.css", "(prefers-color-scheme: light)"),
                    Theme.Parse("prism", Version.Parse("1.29.0"), "prism.dark.css", "(prefers-color-scheme: dark)")
                ],
            };
            File.WriteAllText(ConfigFile, JsonSerializer.Serialize(defaultConfig, JsonSerializerOptions));
        }
        Config = JsonSerializer.Deserialize<Config>(File.ReadAllText(ConfigFile), JsonSerializerOptions);
    }

    internal static void RefreshGroup()
    {
        if (!File.Exists(GroupFile))
        {
            File.WriteAllText(GroupFile, JsonSerializer.Serialize(new GroupConfig
            {
                Current = "czd",
                Groups = new()
                {
                    new() { Id = "czd", Name = "Czompi", Url = "https://czompi.hu" },
                    new() { Id = "czs", Name = "Czompi Software", Url = "https://czsoft.hu" },
                }
            }, JsonSerializerOptions));
        }
        Group = JsonSerializer.Deserialize<GroupConfig>(File.ReadAllText(GroupFile), JsonSerializerOptions);
    }

    //TODO: Optimize! Maybe only load file from disk when change occur and store it in memory (probably precompile parts of the code and replace the Lua code with an id that refers to the id of the compiled code.
    internal static void RefreshPages()
    {
        Pages.Clear();
        if (!Directory.Exists(ContentDirectory))
        {
            Pages.Add("directory-not-found.error", new() { Id = "index", Title = "Main page" });
            return;
        }
        foreach (var file in Directory.GetFiles(ContentDirectory, "*.xmdl", SearchOption.AllDirectories))
        {
            var xmdlContent = File.ReadAllText(file);
            var checksum = SHA1.Encode(xmdlContent);
            if (Pages.TryGetValue(file, out var page))
            {
                if (checksum == page.Checksum) continue;
            }
            
            var xmdlHeader = $"{xmdlContent[0..(xmdlContent.IndexOf("</metadata>", StringComparison.OrdinalIgnoreCase) + "</metadata>".Length)]}";
            var metadata = xmdlHeader.ParseXml<Metadata>();
            metadata.Checksum = checksum;
            
            if (!Pages.TryAdd(file, metadata))
            {
                Pages[file] = metadata;
            }
        }
        RefreshNavbarHierarchy();
    }

    internal static void RefreshNavbarHierarchy()
    {
        NavHierarchy = new NavigationHierarchy();
        foreach (var page in Pages.Values)
        {
            // Check if IsMember is set (it is only necessary when a legacy Xmdl file is present where IsNavMenuItem is used)
            if (!page.Navbar.IsMember && page.IsNavMenuItem) {
                page.Navbar.IsMember = page.IsNavMenuItem;
                // Since the legacy Xmdl file structure didn't allow categorizing the files, it safe to assume that all of these files will be at the root of the hierarchy.
            }
            if (!page.Navbar.IsMember) continue;
            NavHierarchy.BuildHierarchy(page.Navbar.Route.Replace("[]", $"[{page.Title}]"), page.Id);
        }
    }

    public static NavigationHierarchy NavHierarchy { get; internal set; }

    #endregion
}
