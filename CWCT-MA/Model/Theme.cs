namespace CwctMa.Model;

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