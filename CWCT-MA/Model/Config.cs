using Markdig;
using System.Collections.Generic;
using System.IO;

namespace CWCTMA.Model
{
    public class Config
    {
        public static MarkdownPipeline MarkdownPipeline { get; internal set; }
        public string Id { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string SiteURL { get; set; }
        public Meta Meta { get; set; }

        public IEnumerable<Page> Pages { get; set; }

        public static class Files
        {
            public static string WorkingDirectory => Path.GetFullPath("../");
            public static string DataDirectory
            {
                get
                {
                    var dir = Path.GetFullPath(Path.Combine(WorkingDirectory, "data"));

                    Directory.CreateDirectory(dir);

                    return dir;
                }
            }
            public static string ContentDirectory
            {
                get
                {
                    var dir = Path.GetFullPath(Path.Combine(DataDirectory, "content"));

                    Directory.CreateDirectory(dir);

                    return dir;
                }
            }
        }
    }
}