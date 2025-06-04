using Markdig;
using System.Collections.Generic;
using System.IO;

namespace CzSoft.CwateMa.Model;

public class Config
{
    public string Id { get; set; }
    public string CdnUrl { get; set; }
    public bool HideHeader { get; set; } = false;
    public bool HideFooter { get; set; } = false;
    public string ShortName { get; set; }
    public string FullName { get; set; }
    public string SiteURL { get; set; }
    public string GlobalUrl { get; set; } = "https://czompigroup.hu/";
    public string CopyrightHolder { get; set; } = "Czompi Software";
    public string CopyrightUrl { get; set; } = "https://czsoft.hu/";
    public string LokiUrl { get; set; } = "http://localhost:3100";
    public List<Theme> Themes { get; set; } = [];
    public Meta Meta { get; set; }
}
