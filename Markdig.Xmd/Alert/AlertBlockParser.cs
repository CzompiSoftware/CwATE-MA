using Markdig.Xmd.CSCode;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Markdig.Xmd.Alert
{
    /// <summary>
    /// An inline parser for <see cref="AlertBlock"/>.
    /// </summary>
    /// <seealso cref="InlineParser" />
    /// <seealso cref="IPostInlineProcessor" />
    public class AlertBlockParser : InlineParser
    {
        private MarkdownPipelineBuilder? _pipeline;

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
        public AlertBlockParser()
        {
            OpeningCharacterString = "]";
            OpeningCharacters = new char[] { OpeningCharacterString[0] };
            DefaultClass = "alert";
        }

        public AlertBlockParser(MarkdownPipelineBuilder pipeline) : base()
        {
            _pipeline = pipeline;
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            //if (slice.PeekCharExtra(1) != OpeningCharacterString[1]) return false;

            string text = slice.Text;
            if (string.IsNullOrEmpty(text)) return false;
            if (!text.StartsWith(OpeningCharacterString)) return false;
            //if (!text.Contains(ClosingCharacterString)) return false;

            var textList = text.Split($"\n").ToList();
            for (int i = 0; i < textList.Count; i++)
            {

                if (!textList[i].StartsWith(OpeningCharacterString)) return false;
                textList[i] = textList[i].Replace(OpeningCharacterString, "");
            }


            string type = "";
            var firstLine = textList[0];
            int typeStart = firstLine.IndexOf(">"), typeEnd = firstLine.IndexOf("<");
            if (firstLine.StartsWith(">") && textList[0].Contains("<"))
            {
                type = firstLine[(typeStart + 1)..typeEnd];
                textList[0] = textList[0][(typeEnd + 1)..];
            }

            int start = text.IndexOf(OpeningCharacterString);

            if (start == -1) return false;
            string content = string.Join("\n", textList);
            for (int i = 0; i < $"{text}".Length; i++)
            {
                slice.NextChar();
            }
            processor.Inline = new AlertBlock(_pipeline) { Content = content, Type = type };
            processor.Inline.Span = new SourceSpan() { Start = processor.GetSourcePosition(slice.Start, out int line, out int column) };
            processor.Inline.Line = line;
            processor.Inline.Span.End = processor.Inline.Span.Start + $"{text}".Length;
            return true;
        }
    }
}
