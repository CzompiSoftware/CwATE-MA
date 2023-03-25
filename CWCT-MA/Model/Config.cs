using Markdig;
using System.Collections.Generic;
using System.IO;

namespace CwctMa.Model;

public class Config
{
    public string CdnUrl { get; set; }
    public string Id { get; set; }
    public string ShortName { get; set; }
    public string FullName { get; set; }
    public string SiteURL { get; set; }
    public string GlobalUrl { get; set; } = "https://czompigroup.hu/";
    public string CopyrightHolder { get; set; } = "Czompi Software";
    public string CopyrightUrl { get; set; } = "https://czsoft.hu/";
    public string LokiUrl { get; set; } = "http://localhost:3100";
    public List<Theme> Themes { get; set; } = new()
    {
        new() { Name = "cwctma", Version = "<#={app_version}#>" },
        new() { Name = "cwctma", Version = "<#={app_version}#>", FileName = "style.<#={site_id}#>.css" }
    };
    public Meta Meta { get; set; }
}
