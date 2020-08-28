using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ScrollStitch.V20200707.Text
{
    public class MultiLineTextOutput
        : IMultiLineTextOutput
    {
        #region private
        private static readonly string _newLine = Environment.NewLine;
        private static readonly int _newLineCharCount = _newLine.Length;
        private int _length;
        private List<string> _lines;
        #endregion

        public IReadOnlyList<string> Lines => _lines.AsReadOnly();

        public int LineCount => _lines.Count;

        public int Length => (_length + _lines.Count * _newLineCharCount);

        public MultiLineTextOutput()
        {
            _lines = new List<string>();
        }

        public void AppendLine()
        {
            _lines.Add(string.Empty);
        }

        public void AppendLine(string s)
        {
            _lines.Add(s);
            _length += s.Length;
        }

        public void AppendLines(IEnumerable<string> lines)
        {
            foreach (var line in lines)
            {
                AppendLine(line);
            }
        }

        public void ParseMultiLine(string multiLine)
        {
            string[] separators = new string[]
            {
                _newLine
            };
            ;
            foreach (var line in multiLine.Split(separators, StringSplitOptions.None))
            {
                AppendLine(line);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            ToStringBuilder(sb, 100, 48 * 1024 - 256);
            return sb.ToString();
        }

        public void ToConsole()
        {
            foreach (var line in _lines)
            {
                Console.WriteLine(line);
            }
        }

        public void ToTextWriter(TextWriter textWriter)
        {
            foreach (var line in _lines)
            {
                textWriter.WriteLine(line);
            }
        }

        public void ToStringBuilder(StringBuilder sb)
        {
            int estimate = _length + _lines.Count * _newLineCharCount;
            int newLength = sb.Length + estimate;
            sb.EnsureCapacity(newLength);
            foreach (var line in _lines)
            {
                sb.AppendLine(line);
            }
        }

        public void ToStringBuilder(StringBuilder sb, int maxLineCount)
        {
            ToStringBuilder(sb, maxLineCount, maxCharCount: int.MaxValue);
        }

        public void ToStringBuilder(StringBuilder sb, int maxLineCount, int maxCharCount)
        {
            //
            // TODO WARNING
            // The following code has not yet been checked for correctness.
            //
            int lineCount = _lines.Count;
            int includedWholeLineCount = 0;
            int charsToTruncateForLastLine = -1;
            int estimate = 0;
            for (int lineIndex = 0; lineIndex < lineCount; ++lineIndex)
            {
                int currLength = _lines[lineIndex].Length;
                if (estimate + currLength + 3 + 2 * _newLineCharCount <= maxCharCount)
                {
                    ++includedWholeLineCount;
                    estimate += currLength + _newLineCharCount;
                    continue;
                }
                else if (estimate + 3 + _newLineCharCount <= maxCharCount)
                {
                    int currCanPrint = maxCharCount - 3 - estimate - _newLineCharCount;
                    charsToTruncateForLastLine = Math.Max(0, currLength - currCanPrint);
                    estimate += currLength - charsToTruncateForLastLine + 3 + _newLineCharCount;
                    break;
                }
                else 
                {
                    break;
                }
            }
            sb.EnsureCapacity(sb.Length + estimate);
            for (int lineIndex = 0; lineIndex < includedWholeLineCount; ++lineIndex)
            {
                sb.AppendLine(_lines[lineIndex]);
            }
            if (charsToTruncateForLastLine >= 0 &&
                includedWholeLineCount + 1 < _lines.Count)
            {
                var s = _lines[includedWholeLineCount + 1];
                s = s.Substring(0, s.Length - charsToTruncateForLastLine) + "...";
                sb.AppendLine(s);
            }
        }

        public void CopyTo(IMultiLineTextOutput dest)
        {
            foreach (string line in _lines)
            {
                dest.AppendLine(line);
            }
        }

        public string[] ToArray()
        {
            return _lines.ToArray();
        }
    }
}
