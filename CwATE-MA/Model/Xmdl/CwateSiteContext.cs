namespace CzSoft.CwateMa.Model.Xmdl;

public class CwatePageContext
{
    public CwatePageContext()
    {
    }

    public CwatePageContext(string url, string route, string query)
    {
        Url = url;
        Route = route;
        Query = query;
    }

    public string Url { get; set; }
    public string Route { get; set; }
    public string Query { get; set; }
}