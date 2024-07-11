using System;
using $(RootNamespace).Model;

namespace $(RootNamespace);

partial class GeneratedMetadata : IMetadata
{
    public GeneratedMetadata()
    {
        Name = "$(ShortName)";
        FullName = "$(FullName)";
        Version = Version.Parse("$(Version)");
        CompileTime = DateTime.Parse("$([System.DateTime]::UtcNow.ToString())");
        BuildId = Guid.Parse("$([System.Guid]::NewGuid())");
    }
}