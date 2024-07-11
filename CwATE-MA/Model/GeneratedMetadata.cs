namespace CzSoft.CwateMa.Model;

public partial class GeneratedMetadata : IMetadata
{
    public string Name { get; init; }

    public string FullName { get; init; }

    public Version Version { get; init; }

    public DateTime CompileTime { get; init; }

    public Guid BuildId { get; init; }
}