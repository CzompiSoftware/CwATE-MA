using System.Collections.Generic;

namespace CwctMa.Model;

public struct Page
{
    public string Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<string> Alias { get; set; }
}