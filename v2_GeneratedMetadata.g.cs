using System;
using $(RootNamespace).Model;

namespace $(RootNamespace);

partial class GeneratedMetadata : IMetadata
{
    public string Name => "$(ShortName)";
    public string FullName => "$(FullName)";
    public Version Version => Version.Parse("$(Version)");
    public DateTime CompileTime => DateTime.Parse("$([System.DateTime]::UtcNow.ToString())");
    public Guid BuildId => Guid.Parse("$([System.Guid]::NewGuid())");
}