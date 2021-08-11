using System.IO;
using System.Text.Json;

namespace CWCTMA.Model
{
    public class Globals
    {
        public static string CurrentPage { get; set; }
#if RELEASE
        public static string CDN => "https://cdn.czompisoftware.hu/";
#else
        public static string CDN => "https://cdn.czompisoftware.dev/";
        //public static string CDN = "https://127.0.0.1:42069/";
#endif
        public static Config Config { get; internal set; }
        public static ApplicationMetadata AppMeta { get; internal set; }
        public static string DataDirectory
        {
            get
            {
                var dir = Path.GetFullPath(Path.Combine("..", "data"));
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }
        public static string PagesDirectory
        {
            get
            {
                var dir = Path.GetFullPath(Path.Combine(DataDirectory, "pages"));
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }
        public static string ConfigFile => Path.Combine(DataDirectory, "config.json");
        public static string GroupFile => Path.Combine(DataDirectory, "group.json");

        public static JsonSerializerOptions JsonSerializerOptions => new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReadCommentHandling = JsonCommentHandling.Skip,
            WriteIndented = true
        };

        public static GroupConfig Group { get; internal set; }
    }
}
