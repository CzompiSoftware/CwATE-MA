using CwctMa.Helpers;
using CwctMa.Model.Xmd;
using Markdig;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace CwctMa.Model;

public class Globals
{
    #region Properties

    public static MarkdownPipeline MarkdownPipeline { get; internal set; }

    public static string CurrentPage { get; set; }

    public static ApplicationMetadata AppMeta { get; internal set; }
    public static string AppVersionString => AppMeta.Version.ToString(3);

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
    public static List<Metadata> Pages { get; internal set; } = new();
    #endregion

    #region Methods
    //public static Metadata GetMetadata(string pageId) => Pages?.Page.FirstOrDefault(x => x.Id.Equals(pageId, StringComparison.OrdinalIgnoreCase)) ?? new() { };

    internal static void LoadConfigs()
    {
        RefreshConfig();
        RefreshGroup();
        RefreshPages();
        AppMeta = new()
        {
            Id = Dns.GetHostName(),
            Name = "CWCT/MA",
            FullName = "Czompi WebAPP Common Template for Microsoft ASP.NET",
            Version = Assembly.GetExecutingAssembly().GetName().Version,
            CompileTime = CwctMa.Builtin.CompileTime,
            BuildId = CwctMa.Builtin.BuildId
        };
    }

    internal static void RefreshConfig()
    {
        if (!File.Exists(ConfigFile))
        {
            File.WriteAllText(ConfigFile, JsonSerializer.Serialize(new Config
            {
                Id = "CwctMaDE".ToLower(),
                ShortName = "CWCT/MA DE",
                FullName = "Czompi WebAPP Common Template for Microsoft ASP.NET - Development Environment",

#if RELEASE
                CdnUrl = "https://cdn.czsoft.hu/",
#else
                CdnUrl = "https://cdn.czsoft.dev/",
#endif
                SiteURL = "./",
                GlobalUrl = "https://czompigroup.hu/",
                Meta = new()
                {
                    Title = "CWCT/MA DE",
                    Description = "Czompi WebAPP Common Template for Microsoft ASP.NET - Development Environment",
                    Image = null,
                    PrimaryColor = "#EAEAEA"
                }
            }, JsonSerializerOptions));
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

    //TODO: Rework! Maybe only load file from disk when chage occurred and store it in memory (prolly precompile parts of the code and replace the C# code with an id that refers to the id of the compiled code.
    // Magyarul a vége: Precompile-olja a fájlban található kódrészleteket és készít hozzájuk azonosítókat, majd ezekre az azonosítókra cseréli a tényleges kódot => nem kell mindig recompile-olni.
    internal static void RefreshPages()
    {
        Pages.Clear();
        if (!Directory.Exists(ContentDirectory))
        {
            Pages = new List<Metadata>()
            {
                new() { Id = "index", Title = "Main page" }
            };
            return;
        }
        foreach (var file in Directory.GetFiles(ContentDirectory, "*.xmd"))
        {
            var markdownFile = File.ReadAllText(file);
            var header = $"{markdownFile[0..(markdownFile.IndexOf("</metadata>", StringComparison.OrdinalIgnoreCase) + "</metadata>".Length)]}";
            Pages.Add(header.ParseXml<Metadata>());
        }
    }
    #endregion
}
