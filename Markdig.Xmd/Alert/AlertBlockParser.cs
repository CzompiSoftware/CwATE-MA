using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Markdig.Extensions.Xmd.Alert;

/// <summary>
/// An inline parser for <see cref="AlertBlock"/>.
/// </summary>
/// <seealso cref="InlineParser" />
/// <seealso cref="IPostInlineProcessor" />
public class AlertBlockParser : InlineParser
{
    private MarkdownPipelineBuilder _pipeline;

    public string OpeningCharacterString { get; }
    public string ClosingCharacterString { get; }
    public char[] ClosingCharacters { get; }

    /// <summary>
    /// Gets or sets the default class to use when creating a math inline block.
    /// </summary>
    public string DefaultClass { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlertBlock"/> class.
    /// </summary>

    public AlertBlockParser(MarkdownPipelineBuilder pipeline)
    {
        OpeningCharacterString = "]";
        OpeningCharacters = new char[] { OpeningCharacterString[0] };
        DefaultClass = "alert";
        _pipeline = pipeline;
    }

    public override bool Match(InlineProcessor processor, ref StringSlice slice)
    {
        //if (slice.PeekCharExtra(1) != OpeningCharacterString[1]) return false;

        string text = slice.Text;
        if (string.IsNullOrEmpty(text)) return false;
        if (!text.StartsWith($"{OpeningCharacterString}>")) return false;
        //if (!text.Contains(ClosingCharacterString)) return false;
        var (type, content) = RenderContent(text);

        if (content == null) return false;

        for (int i = 0; i < text.Length; i++)
        {
            slice.NextChar();
        }

        processor.Inline = new AlertBlock(_pipeline) { Content = content, Type = type };
        processor.Inline.Span = new SourceSpan() { Start = processor.GetSourcePosition(slice.Start, out int line, out int column) };
        processor.Inline.Line = line;
        processor.Inline.Span.End = processor.Inline.Span.Start + text.Length;
        return true;
    }

    private (string, string) RenderContent(string text)
    {
        var regex = new Regex("\\]>(.*)<");
        var matches = regex.Matches(text);
        var groups = matches.First().Groups.Cast<Group>().ToList();

        if (groups.Count < 2) return (null, null);

        string type = groups[1].Value;
        string[] contentLines = text[groups[0].Length..].Split('\n');
        contentLines = contentLines.Select(x => x.Trim().TrimStart(']').TrimStart(' ').TrimEnd('\r')).ToArray();
        var content = string.Join("\n", contentLines);
        return text.Contains("]>") ? (type, content) : (type, content);
    }
}
