using CzomPack.Cryptography;

namespace CWCTMA.Model
{
    public class CDN
    {
        public static string GetUrl(string loc)
        {
            string[] locArr = loc.Split('/');

            if (locArr.Length < 2) return loc;
            return $"{Globals.CDN}{locArr[0]}/{SHA1.Encode(locArr[1]).ToLower()}/{string.Join('/', locArr[2..])}";
        }
    }
}
