using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Markdig.Extensions.Xmd.CSCode;


/// <summary>
/// A C# source code inline element.
/// </summary>
/// <seealso cref="FencedCodeBlock" />
public class CSCodeBlock : LeafInline
{
    public CSCodeBlock()
    {
    }

    /// <summary>
    /// The trimmed source code.
    /// </summary>
    public string SourceCode { get; set; }
}
