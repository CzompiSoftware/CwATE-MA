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
    public List<Theme> Themes { get; set; } = new()
    {
        new() { Name ="cwctma", Version = "<#={app_version}#>" },
        new() { Name ="cwctma", Version = "<#={app_version}#>", FileName ="style.<#={site_id}#>.css" }
    };
    public Meta Meta { get; set; }
}

public class Theme
{
    private string fileName = "master.css";
    private string version;
    private string url = null;

    public string Name { get; set; }

    public string Version
    {
        get => version?
                    .Replace("<#={app_version}#>", Globals.AppVersionString, System.StringComparison.OrdinalIgnoreCase)
                    .Replace("<#={site_id}#>", Globals.Config.Id, System.StringComparison.OrdinalIgnoreCase);
        set => version = value;
    }

    public string FileName
    {
        get => fileName?
                    .Replace("<#={app_version}#>", Globals.AppVersionString, System.StringComparison.OrdinalIgnoreCase)
                    .Replace("<#={site_id}#>", Globals.Config.Id, System.StringComparison.OrdinalIgnoreCase);
        set => fileName = value;
    }

    /// <summary>
    /// It is only used when <see cref="Name"/> and <see cref="Version"/> are null.
    /// Otherwise it'll load the theme from cdn.czsoft.hu.
    /// </summary>
    public string Url
    {
        get => url?
                    .Replace("<#={app_version}#>", Globals.AppVersionString, System.StringComparison.OrdinalIgnoreCase)
                    .Replace("<#={site_id}#>", Globals.Config.Id, System.StringComparison.OrdinalIgnoreCase);
        set => url = value;
    }
    public void Deconstruct(out string name, out string version, out string file_name, out string url)
    {
        name = Name;
        version = Version;
        file_name = FileName;
        url = Url;
    }
}