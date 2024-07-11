namespace CzSoft.CwateMa.Model;

public interface IMetadata
{
    public string Name { get; }
    public string FullName { get; }
    public Version Version { get; }
    public DateTime CompileTime { get; }
    public Guid BuildId { get; }
}