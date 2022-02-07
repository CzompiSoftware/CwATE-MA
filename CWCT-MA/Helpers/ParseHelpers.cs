
using System.IO;
using System.Text.Json;
using System.Xml;
using System.Xml.Serialization;

namespace CwctMa.Helpers;
internal static class ParseHelpers
{
    public static Stream ToStream(this string @this)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(@this);
        writer.Flush();
        stream.Position = 0;
        return stream;
    }


    public static T ParseXml<T>(this string @this) where T : class => new XmlSerializer(typeof(T)).Deserialize(XmlReader.Create(@this.Trim().ToStream(), new XmlReaderSettings() { ConformanceLevel = ConformanceLevel.Document, DtdProcessing = DtdProcessing.Ignore })) as T;
    
    public static void ToFile<T>(this T @this, string fileName) where T : class
    {
        XmlSerializer writer = new(typeof(T));

        using FileStream file = File.Create(fileName);
        writer.Serialize(file, @this);
    }
    public static string ToString<T>(this T @this) where T : class
    {
        using var stringwriter = new StringWriter();
        var serializer = new XmlSerializer(@this.GetType());
        serializer.Serialize(stringwriter, @this);
        return stringwriter.ToString();
    }

    public static T ParseJson<T>(this string @this) where T : class => JsonSerializer.Deserialize<T>(@this.Trim());
}