using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CzSoft.CwateMa.Model;

public static class Extensions
{
    public static string ToFancyString(this Version version) => $"{version.Major}.{version.Minor}{(version.Build > 0 ? $".{version.Build}" : "")}";

    public static string ToSqlTimeString(this DateTime dt) => $"{dt.Year:D4}-{dt.Month:D2}-{dt.Day:D2}T{dt.Hour:D2}:{dt.Minute:D2}"; // ####-##-##T##:##
    public static string GetActualFileName(this string pathAndFileName)
    {
        string directory = Path.GetDirectoryName(pathAndFileName);
        string pattern = Path.GetFileName(pathAndFileName);
        string resultFileName;

        // Enumerate all files in the directory, using the file name as a pattern
        // This will list all case variants of the filename even on file systems that
        // are case sensitive
        var files = Directory.GetFiles(directory, "*.*", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
        var foundFiles = files.Where(x => x.Equals(pathAndFileName, StringComparison.OrdinalIgnoreCase));

        if (foundFiles.Any())
        {
            if (foundFiles.Count() > 1)
            {
                // More than two files with the same name but different case spelling found
                throw new Exception("Ambiguous File reference for " + pathAndFileName);
            }
            else
            {
                resultFileName = foundFiles.First();
            }
        }
        else
        {
            throw new FileNotFoundException("File not found", pathAndFileName);
        }

        return resultFileName;
    }
}
