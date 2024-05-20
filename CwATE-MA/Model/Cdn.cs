using CzomPack.Cryptography;

namespace Cwatema.Model;

public class Cdn
{
    public static string GetUrl(string loc)
    {
        string[] locArr = loc.Split('/');

        if (locArr.Length <= 2) return loc;
        if (locArr.Length > 2 && loc.Contains('@')) return $"{Globals.Config.CdnUrl ?? "https://cdn.czsoft.hu/"}{locArr[0]}/{locArr[1].ToLowerInvariant()}/{string.Join('/', locArr[2..])}";
        if (locArr.Length <= 3) return loc;
        return $"{Globals.Config.CdnUrl ?? "https://cdn.czsoft.hu/"}{locArr[0]}/{locArr[1].ToLowerInvariant()}@{locArr[2].ToLowerInvariant()}/{string.Join('/', locArr[3..])}";
    }

    public static string GetLegacyUrl(string loc)
    {
        string[] locArr = loc.Split('/');

        if (locArr.Length < 2) return loc;
        return $"{Globals.Config.CdnUrl ?? "https://cdn.czsoft.hu/"}{locArr[0]}/{SHA1.Encode(locArr[1]).ToLower()}/{string.Join('/', locArr[2..])}";
    }
}
