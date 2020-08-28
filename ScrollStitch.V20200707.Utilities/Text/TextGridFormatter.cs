using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    public class TextGridFormatter
    {
        public enum Justify
        { 
            Left = 0,
            Center = 1,
            Right = 2,
        }

        public ITextGridSource Source { get; }

        public int RowCount => Source.RowCount;
        
        public int ColumnCount => Source.ColumnCount;
        
        public int[] ColumnMaxLengths { get; private set; }

        public int Indent { get; set; } = 8;

        public int ColumnSpacing { get; set; } = 4;

        public Justify[] ColumnJustify { get; private set; }

        public TextGridFormatter(int rowCount, int columnCount, Func<int, int, string> rowColumnTextFunc, bool asArray = false)
            : this(asArray 
                  ? TextGridSource.Create(_CtorStringsFromFunc(rowCount, columnCount, rowColumnTextFunc))
                  : TextGridSource.Create(rowCount, columnCount, rowColumnTextFunc))
        {
        }

        public TextGridFormatter(string[,] textArray)
            : this(TextGridSource.Create(textArray))
        {
        }

        public TextGridFormatter(ITextGridSource source)
        {
            Source = source;
            _GetColumnJustify();
        }

        public void Generate(IMultiLineTextOutput output)
        {
            _GetColumnStringMaxLength();
            var proc = new LineProcessor(this, output);
            int rowCount = RowCount;
            for (int row = 0; row < rowCount; ++row)
            {
                proc.GenerateOutput(row);
            }
        }

        private static string[,] _CtorStringsFromFunc(int rowCount, int columnCount, Func<int, int, string> rowColumnTextFunc)
        {
            var textArray = new string[rowCount, columnCount];
            for (int row = 0; row < rowCount; ++row)
            {
                for (int col = 0; col < columnCount; ++col)
                {
                    textArray[row, col] = rowColumnTextFunc(row, col);
                }
            }
            return textArray;
        }

        private void _GetColumnStringMaxLength()
        {
            if (!(ColumnMaxLengths is null))
            {
                return;
            }
            int rowCount = RowCount;
            int columnCount = ColumnCount;
            int[] maxLens = new int[columnCount];
            for (int row = 0; row < rowCount; ++row)
            {
                for (int col = 0; col < columnCount; ++col)
                {
                    maxLens[col] = Math.Max(maxLens[col], Source.GetItem(row, col).Length);
                }
            }
            ColumnMaxLengths = maxLens;
        }

        private void _GetColumnJustify()
        {
            if (!(ColumnJustify is null))
            {
                return;
            }
            int columnCount = ColumnCount;
            Justify[] justify = new Justify[columnCount];
            for (int col = 0; col < columnCount; ++col)
            {
                justify[col] = Justify.Right;
            }
            ColumnJustify = justify;
        }

        private class LineProcessor
        {
            private readonly ITextGridSource _source;
            private readonly int[] _maxLens;
            private readonly int _rowCount;
            private readonly int _columnCount;
            private readonly int _indent;
            private readonly int _spacing;
            private readonly Justify[] _justify;
            private IMultiLineTextOutput _output;
            private StringBuilder _sb;

            internal LineProcessor(TextGridFormatter host, IMultiLineTextOutput output)
            {
                _source = host.Source;
                _maxLens = host.ColumnMaxLengths;
                _rowCount = host.RowCount;
                _columnCount = host.ColumnCount;
                _indent = host.Indent;
                _spacing = host.ColumnSpacing;
                _justify = host.ColumnJustify;
                _output = output;
                _sb = new StringBuilder();
            }

            internal void ApplyIndent()
            {
                if (_indent > 0)
                {
                    _sb.Append(' ', _indent);
                }
            }

            internal void AppendJustified(int column, string label)
            {
                var len = label.Length;
                var padCount = _maxLens[column] - len;
                if (padCount == 0)
                {
                    _sb.Append(label);
                }
                else
                {
                    var j = _justify[column];
                    int leftPad, rightPad;
                    switch (j)
                    {
                        case Justify.Left:
                            leftPad = 0;
                            rightPad = padCount;
                            break;
                        case Justify.Center:
                            rightPad = padCount / 2;
                            leftPad = padCount - rightPad;
                            break;
                        case Justify.Right:
                            leftPad = padCount;
                            rightPad = 0;
                            break;
                        default:
                            throw new Exception();
                    }
                    if (leftPad > 0)
                    {
                        _sb.Append(' ', leftPad);
                    }
                    _sb.Append(label);
                    if (rightPad > 0)
                    {
                        _sb.Append(' ', rightPad);
                    }
                }
            }

            internal void ApplySpacing(int column)
            {
                if (column + 1 < _columnCount &&
                    _spacing > 0)
                {
                    _sb.Append(' ', _spacing);
                }
            }

            internal void GenerateOutput(int row)
            {
                ApplyIndent();
                for (int col = 0; col < _columnCount; ++col)
                {
                    string label = _source.GetItem(row, col);
                    AppendJustified(col, label);
                    ApplySpacing(col);
                }
                _output.AppendLine(_sb.ToString());
                _sb.Clear();
            }
        }
    }
}
