namespace CzSoft.CwateMa.Model;


public class ApplicationMetadata : GeneratedMetadata, IMetadata
{
    public static ApplicationMetadata Parse(GeneratedMetadata generatedMetadata, string id = null)
    {
        var metadata = new ApplicationMetadata
        {
            FullName = generatedMetadata.FullName,
            Name = generatedMetadata.Name,
            Version = generatedMetadata.Version,
            CompileTime = generatedMetadata.CompileTime,
            BuildId = generatedMetadata.BuildId,
            Id = id
        };

        return metadata;
    }
    public string Id { get; init; }
}
