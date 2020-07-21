using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ScrollStitch.V20200707.Collections.Specialized
{
    using ScrollStitch.V20200707.Memory;

    /// <summary>
    /// <para>
    /// <see cref="IntKeyIntRow"/> (Integer-keyed Integer Rows) is a dictionary-based collection
    /// with an integer key and rows of integers (fixed-size arrays) as values.
    /// </para>
    /// 
    /// <para>
    /// This class provides several trigger functions for initializing the row data upon creation.
    /// </para>
    /// 
    /// <para>
    /// All integer values are accepted as valid keys. In particular, negative and non-sequential
    /// key values are allowed.
    /// </para>
    /// 
    /// <para>
    /// Information on Thread Safety:
    /// <br/>
    /// This class <b>does not</b> implement any thread-safety feature.
    /// <br/>
    /// Certain retrieval functions may trigger modification behavior. Refer to each function's 
    /// documentation for details.
    /// </para>
    /// 
    /// </summary>
    /// 
    public class IntKeyIntRow
        : ITable<int, int>
    {
        public static class Defaults
        {
            public static int InitialCapacity { get; set; } = 1024;
            public static IArrayPoolClient<int> ArrayPool { get; set; } = ExactLengthArrayPool<int>.DefaultInstance;
        }

        public enum AfterVisit
        { 
            NoChange = 0,
            Update = 1,
            Delete = 2
        }

        /// <summary>
        /// This delegate describes a user-provided function that is called for all purposes.
        /// 
        /// </summary>
        /// <param name="key">
        /// THe key for the row that is to be retrieved and/or updated.
        /// </param>
        /// 
        /// <param name="exists">
        /// True if the key and the row exists prior to the call.
        /// </param>
        /// 
        /// <param name="rowData">
        /// A temporary array used to communicate existing row data and changes to the row data.
        /// If the row does not already exist, the array's content is unspecified.
        /// </param>
        /// 
        /// <returns>
        /// A <see cref="AfterVisit"/> code that specifies what should happen after exiting from 
        /// this function.
        /// <br/>
        /// Returning <see cref="AfterVisit.Update"/> causes the content of <paramref name="rowData"/>
        /// to be copied into the table.
        /// <br/>
        /// Returning <see cref="AfterVisit.Delete"/> causes the key and the row to be deleted.
        /// <br/>
        /// Returning <see cref="AfterVisit.NoChange"/> leaves the table unchanged, irrespective of 
        /// the content of <paramref name="rowData"/>.
        /// </returns>
        /// 
        /// <remarks>
        /// Important. The array is recycled to an object pool after each call. Therefore,
        /// the user-provided function must not retain a reference to the array, or else 
        /// there will be interference to the subsequent operations.
        /// </remarks>
        /// 
        public delegate AfterVisit VisitRowDelegate(int key, bool exists, int[] rowData);

        /// <summary>
        /// This delegate describes a function that is automatically called when a new key and row is
        /// created.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rowData"></param>
        public delegate void NewRowDelegate(int key, int[] rowData);

        /// <summary>
        /// This delegate describes a function that is automatically called when a row is deleted.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="rowData"></param>
        public delegate void DeleteRowDelegate(int key, int[] rowData);

        #region private
        private Dictionary<int, int> _keyMap;
        private int[] _table;
        private int _capacity;
        private int _usage;
        private Stack<int> _removed;
        private Stack<int[]> _spares;
        private IArrayPoolClient<int> _arrayPool;
        #endregion

        public int ColumnCount { get; }

        #region property class OnNewRowBehavior
        /// <summary>
        /// A nested property class that specifies the behavior for the <see cref="IntKeyIntRow"/>
        /// that owns this property.
        /// </summary>
        public class NewRowBehavior
        {
            #region private
            private IntKeyIntRow _host;
            private int[] _onNewRowData;
            #endregion

            /// <summary>
            /// The number of columns in each row in this table.
            /// </summary>
            public int ColumnCount { get; }

            /// <summary>
            /// Specifies a callback that will initialize a new row of data whenever necessary.
            /// 
            /// <para>
            /// Important remarks.
            /// <br/>
            /// If a new row is created via <see cref="WriteRow"/>, this callback will not be 
            /// called, since the caller already provides the complete data for the new row.
            /// <br/>
            /// The <see cref="Data"/> member always takes precedence over <see cref="Callback"/>.
            /// </para>
            /// </summary>
            public NewRowDelegate Callback { get; set; } = null;

            /// <summary>
            /// Specifies a row of data that will be copied into any newly created row upon 
            /// creation.
            /// 
            /// <para>
            /// Important remarks.
            /// <br/>
            /// To unassign this member, set this member to <see cref="null"/>.
            /// <br/>
            /// When assigning an array to this member, its length must equal to 
            /// <see cref="IntKeyIntRow.ColumnCount"/>.
            /// <br/>
            /// The <see cref="Data"/> member always takes precedence over <see cref="Callback"/>.
            /// </para>
            /// 
            /// </summary>
            /// 
            /// <exception cref="ArgumentException">
            /// Incorrect array length.
            /// </exception>
            ///
            public int[] Data
            {
                get => _onNewRowData;
                set => _onNewRowData = _ValidateRowDataElseNull(value);
            }

            public void Clear()
            {
                Callback = null;
                _onNewRowData = null;
            }

            /// <summary>
            /// <para>
            /// Do not call. <br/>Use the <see cref="IntKeyIntRow.OnNewRow"/> property instead.
            /// </para>
            /// </summary>
            /// <param name="host"></param>
            internal NewRowBehavior(IntKeyIntRow host)
            {
                _host = host;
                ColumnCount = host.ColumnCount;
            }

            private int[] _ValidateRowDataElseNull(int[] rowData)
            {
                int len = _host._ValidateRowDataArray(rowData, allowZeroOrNull: true);
                if (len == 0)
                {
                    return null;
                }
                return rowData;
            }
        }
        #endregion

        /// <summary>
        /// Provides a choice of mechanisms to ensure that the entire row is initialized properly
        /// upon new row creation.
        /// </summary>
        public NewRowBehavior OnNewRow { get; private set; }

        /// <summary>
        /// A callback upon row deletion.
        /// </summary>
        public DeleteRowDelegate OnDeleteRow { get; set; } = null;

        public int this[int key, int column]
        {
            get
            {
                if (!TryGetValue(key, column, out int value))
                {
                    throw new KeyNotFoundException();
                }
                return value;
            }
            set
            {
                WriteValue(key, column, value);
            }
        }

        public int[] this[int key]
        {
            get 
            {
                return TryGetRow(key);
            }
            set
            {
                WriteRow(key, value);
            }
        }

        public IntKeyIntRow(int columnCount)
            : this(columnCount, Defaults.InitialCapacity)
        { 
        }

        public IntKeyIntRow(int columnCount, int capacity)
        {
            if (columnCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(columnCount));
            }
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            ColumnCount = columnCount;
            OnNewRow = new NewRowBehavior(this);
            _keyMap = new Dictionary<int, int>(capacity: capacity);
            _capacity = capacity;
            _usage = 0;
            _removed = new Stack<int>(capacity: capacity);
            _spares = new Stack<int[]>(capacity: 16);
            _arrayPool = Defaults.ArrayPool;
            int len = capacity * columnCount;
            if (len > 0)
            {
                _table = _RentTable(len);
            }
        }

        public bool ContainsKey(int key)
        {
            return _keyMap.ContainsKey(key);
        }

        public int[] GetKeys()
        {
            int[] keys = new int[_keyMap.Count];
            _keyMap.Keys.CopyTo(keys, 0);
            return keys;
        }

        public KeyValuePair<int, int[]>[] ToArray()
        {
            int[] keys = GetKeys();
            int keyCount = keys.Length;
            int columnCount = ColumnCount;
            var array = new KeyValuePair<int, int[]>[keyCount];
            for (int k = 0; k < keyCount; ++k)
            {
                int key = keys[k];
                int[] rowData = new int[columnCount];
                int rowId = _keyMap[key];
                int tableOffset = rowId * columnCount;
                Array.Copy(_table, tableOffset, rowData, 0, columnCount);
                array[k] = new KeyValuePair<int, int[]>(key, rowData);
            }
            return array;
        }

        public int[,] ToArray(int[] keys, int[] columns)
        {
            int columnCount = ColumnCount;
            int requestRowCount = keys.Length;
            int requestColumnCount = columns.Length;
            for (int k = 0; k < requestColumnCount; ++k)
            {
                int column = columns[k];
                if (column < 0 ||
                    column >= columnCount)
                {
                    throw new IndexOutOfRangeException(
                        message: $"Columns array contains invalid value: {column}");
                }
            }
            int[] rowIds = new int[requestRowCount];
            int[] tableOffsets = new int[requestRowCount];
            for (int k = 0; k < requestRowCount; ++k)
            {
                int key = keys[k];
                if (!_keyMap.TryGetValue(key, out int rowId))
                {
                    throw new IndexOutOfRangeException(
                        message: $"Keys array contains invalid value: {key}");
                }
                int tableOffset = rowId * columnCount;
                rowIds[k] = rowId;
                tableOffsets[k] = tableOffset;
            }
            var array = new int[requestRowCount, requestColumnCount];
            for (int r = 0; r < requestRowCount; ++r)
            {
                int tableOffset = tableOffsets[r];
                for (int c = 0; c < requestColumnCount; ++c)
                {
                    int column = columns[c];
                    array[r, c] = _table[tableOffset + column];
                }
            }
            return array;
        }

        /// <summary>
        /// Writes a row of data to the specified key. If the key does not already exist, it will 
        /// be created and associated with a new row.
        /// 
        /// Remark. This method does not call <see cref="OnNewRow"/>, because the caller of this 
        /// method will provide the entire row of data. This eliminates the need for initialization.
        /// </summary>
        /// <param name="key">The key for the row to be written.</param>
        /// <param name="rowData">The data for the row to be written.</param>
        /// 
        public void WriteRow(int key, int[] rowData)
        {
            _ValidateRowDataArray(rowData, allowZeroOrNull: false);
            bool exists = _keyMap.TryGetValue(key, out int rowId);
            if (!exists)
            {
                rowId = _NewRow();
                _keyMap.Add(key, rowId);
            }
            int columnCount = ColumnCount;
            int tableOffset = rowId * columnCount;
            Array.Copy(rowData, 0, _table, tableOffset, columnCount);
        }

        /// <summary>
        /// Retrieves the specified row of data, if it exists.
        /// </summary>
        /// <param name="key">
        /// The key for the row to be retrieved.
        /// </param>
        /// <param name="rowData">
        /// An array for holding the retrieved row data. The caller must allocate this array with 
        /// length equal to <see cref="ColumnCount"/>.
        /// </param>
        /// <returns>
        /// True if the row of data exists. Upon returning true, <paramref name="rowData"/> will contain 
        /// a copy of the row data. Upon return false, the content of <paramref name="rowData"/> is unchanged.
        /// </returns>
        /// 
        public bool TryGetRow(int key, int[] rowData)
        {
            if (!_keyMap.TryGetValue(key, out int rowId))
            {
                return false;
            }
            _ValidateRowDataArray(rowData, allowZeroOrNull: false);
            int columnCount = ColumnCount;
            int tableOffset = rowId * columnCount;
            Array.Copy(_table, tableOffset, rowData, 0, columnCount);
            return true;
        }

        /// <summary>
        /// Retrieves the specified row of data, if it exists.
        /// </summary>
        /// <param name="key">
        /// The key for the row to be retrieved.
        /// </param>
        /// <returns>
        /// An array of length <see cref="ColumnCount"/> if the key for the row exists. <br/>
        /// Null if the key and the row does not exist.
        /// </returns>
        /// 
        public int[] TryGetRow(int key)
        {
            if (!_keyMap.TryGetValue(key, out int rowId))
            {
                return null;
            }
            int columnCount = ColumnCount;
            int tableOffset = rowId * columnCount;
            int[] rowData = new int[columnCount];
            Array.Copy(_table, tableOffset, rowData, 0, columnCount);
            return rowData;
        }

        /// <summary>
        /// Writes a single value to the specified column on the row corresponding to the key.
        /// 
        /// <para>
        /// If the key does not already exist, the key and the row will be created.
        /// The row data will be initialized with a call to <see cref="OnNewRow"/>.
        /// If that function is not specified, the row's other columns may contain 
        /// uninitialized values.
        /// </para>
        /// </summary>
        /// 
        /// <param name="key">The key for the row to be modified.</param>
        /// <param name="column">The column on the row to be modified.</param>
        /// <param name="value">The value to be written at the row and column.</param>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Invalid column index.
        /// </exception>
        /// 
        public void WriteValue(int key, int column, int value)
        {
            _ValidateColumn(column);
            bool exists = _keyMap.TryGetValue(key, out int rowId);
            if (!exists)
            {
                rowId = _NewRow();
                _keyMap.Add(key, rowId);
            }
            int columnCount = ColumnCount;
            int tableOffset = rowId * columnCount;
            if (!exists)
            {
                if (!(OnNewRow.Data is null))
                {
                    Array.Copy(OnNewRow.Data, 0, _table, tableOffset, columnCount);
                }
                else if (!(OnNewRow.Callback is null))
                {
                    int[] spare = _RentSpare();
                    OnNewRow.Callback(key, spare);
                    Array.Copy(spare, 0, _table, tableOffset, columnCount);
                    _ReturnSpare(spare);
                }
            }
            _table[tableOffset + column] = value;
        }

        /// <summary>
        /// Retrieves a single value from the specified column on the row corresponding to the key,
        /// if the key already exists.
        /// </summary>
        /// 
        /// <param name="key"></param>
        /// <param name="column"></param>
        /// 
        /// <param name="value">
        /// The output parameter for the retrieved value.
        /// </param>
        /// 
        /// <returns>
        /// True if the row key exists. Upon returning true, <paramref name="value"/> will contain
        /// the retrieved value.
        /// <br/>
        /// False if the row key does not exist. Upon returning false, the value of 
        /// <paramref name="value"/> is unspecified.
        /// </returns>
        /// 
        /// <exception cref="ArgumentOutOfRangeException">
        /// Invalid column index.
        /// </exception>
        /// 
        public bool TryGetValue(int key, int column, out int value)
        {
            _ValidateColumn(column);
            if (!_keyMap.TryGetValue(key, out int rowId))
            {
                value = default;
                return false;
            }
            int columnCount = ColumnCount;
            int tableOffset = rowId * columnCount;
            value = _table[tableOffset + column];
            return true;
        }

        /// <summary>
        /// Retrieves a single value from the specified column on the row corresponding to the key,
        /// if the key already exists.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="column"></param>
        /// <returns>
        /// A <see cref="Nullable{int}"/>. If the row key exists, the retrieved value is returned. Otherwise, 
        /// the return value is null.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Invalid column index.
        /// </exception>
        public int? TryGetValue(int key, int column)
        {
            _ValidateColumn(column);
            if (!_keyMap.TryGetValue(key, out int rowId))
            {
                return null;
            }
            int tableOffset = rowId * ColumnCount;
            int value = _table[tableOffset + column];
            return value;
        }

        /// <summary>
        /// Call the specified callback function with the row key and data. <br/>
        /// If the row key does not exist it is automatically created. 
        /// </summary>
        /// <param name="key">
        /// The row key.
        /// </param>
        /// 
        /// <param name="rowAction">
        /// A callback function that will receive a copy of the row data. <br/>
        /// Upon the exit of the callback function, it may specify a follow-up action such as upate or 
        /// delete via an <see cref="AfterVisit"/> enum.
        /// </param>
        /// 
        public void Visit(int key, VisitRowDelegate rowAction)
        {
            int columnCount = ColumnCount;
            bool exists = _keyMap.TryGetValue(key, out int rowId);
            if (!exists)
            {
                rowId = _NewRow();
                _keyMap.Add(key, rowId);
            }
            int tableOffset = rowId * columnCount;
            int[] spare = _RentSpare();
            if (exists)
            {
                Array.Copy(_table, tableOffset, spare, 0, columnCount);
            }
            else 
            {
                if ((OnNewRow.Data?.Length ?? 0) == columnCount)
                {
                    Array.Copy(OnNewRow.Data, 0, spare, 0, columnCount);
                }
                else if (!(OnNewRow.Callback is null))
                {
                    OnNewRow.Callback.Invoke(key, spare);
                }
            }
            AfterVisit av = rowAction(key, exists, spare);
            switch (av)
            {
                case AfterVisit.Update:
                    Array.Copy(spare, 0, _table, tableOffset, columnCount);
                    break;
                case AfterVisit.Delete:
                    OnDeleteRow?.Invoke(key, spare);
                    _keyMap.Remove(key);
                    _removed.Push(rowId);
                    break;
                default:
                    break;
            }
            _ReturnSpare(spare);
        }

        /// <summary>
        /// Calls <see cref="Visit(int, VisitRowDelegate)"/> on every row in the table.
        /// </summary>
        /// <param name="rowAction"></param>
        public void VisitAll(VisitRowDelegate rowAction)
        {
            // A copy of the key set is made, to avoid simultaneous modification
            // (rowAction might modify rows while visiting.)
            int[] keys = _keyMap.Keys.ToArray();
            foreach (int key in keys)
            {
                Visit(key, rowAction);
            }
        }

        public void Remove(int key)
        {
            if (!_keyMap.TryGetValue(key, out int rowId))
            {
                return;
            }
            if (OnDeleteRow is null)
            {
                _keyMap.Remove(key);
                _removed.Push(rowId);
                return;
            }
            int columnCount = ColumnCount;
            int tableOffset = rowId * columnCount;
            int[] spare = _RentSpare();
            Array.Copy(_table, tableOffset, spare, 0, columnCount);
            OnDeleteRow(key, spare);
            _keyMap.Remove(key);
            _removed.Push(rowId);
            _ReturnSpare(spare);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int _NewRow()
        {
            if (_removed.Count > 0) return _removed.Pop();
            ++_usage;
            _CheckGrow();
            return (_usage - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _CheckGrow()
        {
            if (_usage <= _capacity) return;
            int columnCount = ColumnCount;
            int newCap = _capacity * 2;
            int newLen = newCap * columnCount;
            int copyLen = _capacity * columnCount;
            int[] oldTable = _table;
            int[] newTable = _RentTable(newLen);
            if (copyLen > 0)
            {
                Array.Copy(oldTable, 0, newTable, 0, copyLen);
            }
            _table = newTable;
            _capacity = newCap;
            if ((oldTable?.Length ?? 0) > 0)
            {
                _ReturnTable(oldTable);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int[] _RentSpare()
        {
            if (_spares.Count > 0)
            {
                return _spares.Pop();
            }
            return new int[ColumnCount];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _ReturnSpare(int[] spare)
        {
#if false
            for (int k = 0; k < spare.Length; ++k)
            {
                spare[k] = 0;
            }
#endif
            _spares.Push(spare);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int[] _RentTable(int newLen)
        {
            if (_arrayPool is null)
            {
                return new int[newLen];
            }
            return _arrayPool.Rent(newLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _ReturnTable(int[] oldTable)
        {
            if (!(_arrayPool is null))
            {
                _arrayPool.Return(oldTable);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _ValidateColumn(int column)
        {
            if (column < 0 ||
                column >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException(
                    paramName: nameof(column),
                    message: "Invalid column index. " + 
                    $"Column count: {ColumnCount}. " + 
                    $"invalid column index: {column}.");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int _ValidateRowDataArray(int[] rowData, bool allowZeroOrNull)
        {
            if (rowData is null)
            {
                if (allowZeroOrNull)
                {
                    return 0;
                }
                throw new ArgumentNullException(paramName: "rowData");
            }
            int length = rowData.Length;
            if (length == ColumnCount)
            {
                return length;
            }
            if (length == 0 && 
                allowZeroOrNull)
            {
                return 0;
            }
            throw new ArgumentException(
                paramName: "rowData",
                message: $"Expects array length same as ColumnCount = {ColumnCount}. " +
                $"Actual array length: {length}.");
        }

        public void Clear()
        {
            _keyMap.Clear();
            int[] oldTable = _table;
            _table = new int[0];
            _capacity = 0;
            _usage = 0;
            _removed.Clear();
            _spares.Clear();
            OnNewRow.Clear();
            OnDeleteRow = null;
            _ReturnTable(oldTable);
        }

        public void Dispose()
        {
            Clear();
        }
    }
}
