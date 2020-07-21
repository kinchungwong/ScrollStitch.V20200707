using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    /// <summary>
    /// A small interface for building a multi-line text output.
    /// </summary>
    public interface IMultiLineTextOutput
    {
        /// <summary>
        /// Estimated total length of text in characters.
        /// </summary>
        int Length { get; }

        int LineCount { get; }

        IReadOnlyList<string> Lines { get; }

        void AppendLine();

        void AppendLine(string s);

        void AppendLines(IEnumerable<string> lines);

        void ParseMultiLine(string multiLine);

        void ToConsole();

        void ToTextWriter(TextWriter textWriter);

        void ToStringBuilder(StringBuilder sb);

        void ToStringBuilder(StringBuilder sb, int maxLineCount);

        void ToStringBuilder(StringBuilder sb, int maxLineCount, int maxCharCount);

        void CopyTo(IMultiLineTextOutput dest);

        string[] ToArray();
    }
}
