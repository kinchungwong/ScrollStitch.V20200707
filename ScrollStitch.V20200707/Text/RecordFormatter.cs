using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Text
{
    public interface IRecordFormatterSource
    { 
        int RowCount { get; }

        int ColumnCount { get; }

        string GetItemString(int row, int column);

        string GetColumnLabel(int column);
    }

    public class RecordFormatterSource<RecordType>
        : IRecordFormatterSource
    { 
        public IReadOnlyList<RecordType> Records { get; }

        public int RowCount => Records.Count;

        public int ColumnCount => _fieldNames.Count;

        #region private
        private List<Func<RecordType, string>> _fieldToStringFuncs { get; } = new List<Func<RecordType, string>>();

        private List<string> _fieldNames { get; } = new List<string>();
        #endregion

        public RecordFormatterSource(IReadOnlyList<RecordType> records)
        {
            Records = records;
        }

        public RecordFormatterSource(IEnumerable<RecordType> records)
        {
            switch (records)
            {
                case RecordType[] arr:
                    Records = arr;
                    break;
                case List<RecordType> list:
                    Records = list;
                    break;
                case IReadOnlyList<RecordType> rolist:
                    Records = rolist;
                    break;
                case ICollection<RecordType> coll:
                    Records = new List<RecordType>(coll).AsReadOnly();
                    break;
                default:
                    Records = new List<RecordType>(records).AsReadOnly();
                    break;
            }
        }

        public RecordFormatterSource<RecordType> AddField(string fieldName, Func<RecordType, string> fieldToStringFunc)
        {
            _fieldNames.Add(fieldName);
            _fieldToStringFuncs.Add(fieldToStringFunc);
            return this;
        }

        public string GetItemString(int row, int column)
        {
            return _fieldToStringFuncs[column](Records[row]);
        }

        public string GetColumnLabel(int column)
        {
            return _fieldNames[column];
        }
    }

    public class RecordFormatter
    {
        public IRecordFormatterSource Source { get; }

        public int Indent { get; set; } = 0;

        public int ColumnSpacing { get; set; } = 1;

        /// <summary>
        /// Specifies whether the row number should be displayed
        /// </summary>
        public bool ShowRowNumber { get; set; } = false;

        public RecordFormatter(IRecordFormatterSource source)
        {
            Source = source;
        }

        public static RecordFormatter Create<RecordType>(
            IEnumerable<RecordType> records, 
            Action<RecordFormatterSource<RecordType>> addFieldsFunc)
        {
            var source = new RecordFormatterSource<RecordType>(records);
            addFieldsFunc(source);
            return new RecordFormatter(source);
        }

        public void Generate(IMultiLineTextOutput output)
        {
            string itemFunc(int row, int col)
            {
                return Source.GetItemString(row, col);
            }
            var rowHeaderFunc = new Func<int, string>((int row) => row.ToString());
            var rowHeaderFunc2 = ShowRowNumber ? rowHeaderFunc : null;
            var source = TextGridSource.Create(Source.RowCount, Source.ColumnCount, itemFunc);
            var source2 = TextGridSource.CreateWithHeaders(source, rowHeaderFunc2, Source.GetColumnLabel);
            TextGridFormatter tgFormatter = new TextGridFormatter(source2);
            tgFormatter.Indent = Indent;
            tgFormatter.ColumnSpacing = ColumnSpacing;
            tgFormatter.Generate(output);
        }
    }
}
