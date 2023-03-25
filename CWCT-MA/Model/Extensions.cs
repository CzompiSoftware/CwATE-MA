using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace CwctMa.Model;

public static class Extensions
{
    public static string ToFancyString(this Version version) => $"{version.Major}.{version.Minor}{(version.Build > 0 ? $".{version.Build}" : "")}";

    public static string ToSqlTimeString(this DateTime dt) => $"{dt.Year:D4}-{dt.Month:D2}-{dt.Day:D2}T{dt.Hour:D2}:{dt.Minute:D2}"; // ####-##-##T##:##
}
