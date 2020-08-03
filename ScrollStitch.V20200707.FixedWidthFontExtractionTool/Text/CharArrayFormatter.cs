using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    public class CharArrayFormatter
    {
        public ICharArray CharArray { get; }

        public CharArrayFormatterOptions Options { get; }

        public CharArrayFormatter(ICharArray charArray)
            : this(charArray, new CharArrayFormatterOptions())
        {
        }

        public CharArrayFormatter(ICharArray charArray, CharArrayFormatterOptions options)
        {
            CharArray = charArray;
            Options = options;
        }

        public void PrintToConsole()
        {
            ToMultiLineText().ToConsole();
        }

        public void PrintToFile(string outputTextFilename)
        {
            // force ASCII encoding in file
            using (var strm = new FileInfo(outputTextFilename).OpenWrite())
            {
                strm.SetLength(0);
                using (var textWriter = new StreamWriter(strm, Encoding.ASCII))
                {
                    PrintToFile(textWriter);
                }
            }
        }

        public void PrintToFile(TextWriter textWriter)
        {
            foreach (var line in ToMultiLineText().Lines)
            {
                textWriter.WriteLine(line);
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            ToMultiLineText().ToStringBuilder(sb, 3);
            sb.AppendLine("...");
            return sb.ToString();
        }

        public MultiLineTextOutput ToMultiLineText()
        {
            MultiLineTextOutput mlto = new MultiLineTextOutput();
            ToMultiLineText(mlto);
            return mlto;
        }

        public void ToMultiLineText(IMultiLineTextOutput mlto)
        {
            _PrintBlankLines(mlto);
            _PrintRows(mlto);
            _PrintBlankLines(mlto);
        }

        private void _PrintBlankLines(IMultiLineTextOutput mlto)
        {
            for (int k = 0; k < Options.BlankLines; ++k)
            {
                mlto.AppendLine();
            }
        }

        private void _PrintRows(IMultiLineTextOutput mlto)
        {
            int rows = CharArray.RowCount;
            for (int row = 0; row < rows; ++row)
            {
                _PrintRow(mlto, row);
            }
        }

        private void _PrintRow(IMultiLineTextOutput mlto, int row)
        {
            int cols = CharArray.ColumnCount;
            int indent = Options.Indentation;
            char[,] charArray = CharArray.CharArray;
            var sb = new StringBuilder();
            sb.Append(' ', indent);
            for (int col = 0; col < cols; ++col)
            {
                sb.Append(charArray[row, col]);
            }
            sb.Append(' ', indent);
            mlto.AppendLine(sb.ToString());
        }
    }
}
