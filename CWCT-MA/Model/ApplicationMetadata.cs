using System;

namespace CwctMa.Model;

public class ApplicationMetadata
{
    public string Name { get; internal set; }
    public string FullName { get; internal set; }
    public Version Version { get; internal set; }
    public DateTime CompileTime { get; internal set; }
    public Guid BuildId { get; internal set; }
    public string Id { get; internal set; }
}