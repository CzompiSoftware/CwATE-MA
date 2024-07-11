using System.Text.Json.Serialization;

namespace CzSoft.CwateMa.Model;

public class Theme
{
    //private string defaultFileName = "master.css";
    //private string version;
    //private string url = null;

    public Theme()
    {

    }
    private Theme(string name, Version version, string fileName, string url, string media) : this()
    {
        Name = name;
        Version = version.ToString(3);
        FileName = fileName;
        Url = url;
        Media = media;
    }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Name { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Version { get; set; }
    //{
    //    get => version?
    //                .Replace("<#={app_version}#>", Globals.AppVersionString, StringComparison.OrdinalIgnoreCase)
    //                .Replace("<#={site_id}#>", Globals.Config?.Id ?? , StringComparison.OrdinalIgnoreCase);
    //    set => version = value;
    //}

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string FileName { get; set; }
    //{
    //    get => defaultFileName?
    //                .Replace("<#={app_version}#>", Globals.AppVersionString, StringComparison.OrdinalIgnoreCase)
    //                .Replace("<#={site_id}#>", Globals.Config.Id, StringComparison.OrdinalIgnoreCase);
    //    set => defaultFileName = value;
    //}

    /// <summary>
    /// It is only used when <see cref="Name"/> and <see cref="Version"/> are null.
    /// Otherwise it'll load the theme from cdn.czsoft.hu.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Url { get; set; }
    //{
    //    get => url?
    //                .Replace("<#={app_version}#>", Globals.AppVersionString, StringComparison.OrdinalIgnoreCase)
    //                .Replace("<#={site_id}#>", Globals.Config.Id, StringComparison.OrdinalIgnoreCase);
    //    set => url = value;
    //}

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Media { get; set; }

    public void Deconstruct(out string name, out string version, out string file_name, out string url)
    {
        name = Name;
        version = Version;
        file_name = FileName;
        url = Url;
    }

    internal static Theme Parse(string name, Version version, string fileName)
    {
        return new Theme(name, version, fileName, null, null);
    }

    internal static Theme Parse(string name, Version version, string fileName = null, string media = null)
    {
        return new Theme(name, version, fileName, null, media);
    }

    internal static Theme Parse(Uri url, string media = null)
    {
        return new Theme(null, null, null, url.ToString(), media);
    }

}