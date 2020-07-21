using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Caching;
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Collections.Specialized;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Imaging;
    using ScrollStitch.V20200707.Imaging.Hash2D;
    using ScrollStitch.V20200707.Logging;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Text;
    using ScrollStitch.V20200707.Utility;

    public class HashPointTable
    {
        #region Internal Diagnostics Options during development only
        public static bool ShouldDiagDump { get; set; } = false;
        #endregion

        /// <summary>
        /// Connects this class to a retrieval-only collection of <see cref="ImageHashPointList"/>
        /// with an integer key (the image ID).
        /// </summary>
        public IItemSource<ImageHashPointList> HashPointSource { get; set; }

        /// <summary>
        /// The list of image keys to be processed by this class.
        /// </summary>
        public UniqueList<int> ImageKeys { get; private set; }

        /// <summary>
        /// A table containing unique Hash2D values found in each image and the index
        /// into the respective image's point list.
        /// </summary>
        private IntKeyIntRow _table;

        /// <summary>
        /// A dictionary with Hash2D values as key, and individual bits indicating that 
        /// the hash value has occurred in the corresponding input image.
        /// </summary>
        private Dictionary<int, uint> _tableColumnFlags;

        public HashPointTable(IItemSource<ImageHashPointList> hashPointSource, IEnumerable<int> imageKeys)
        {
            HashPointSource = hashPointSource;
            ImageKeys = new UniqueList<int>(imageKeys);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Process()
        {
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(Process)}"))
            {
                _ConfigureHashPointTable();
                _PopulateHashPointTable();
                
            }
            if (ShouldDiagDump)
            {
                _DiagDump();
            }
        }

        public void Clear()
        {
            _table.Clear();
            HashPointSource = null;
        }

        public int[] GetHashValues()
        {
            return _table.GetKeys();
        }

        public KeyValuePair<int, int[]>[] GetPointIndexArray()
        {
            return _table.ToArray();
        }

        public int[,] GetPointIndexArray(int[] hashCodeArray, int[] columns)
        {
            return _table.ToArray(hashCodeArray, columns);
        }

        /// <summary>
        /// Return a subset of row keys and their column flags that is filtered using the 
        /// provided predicate function.
        /// </summary>
        /// <param name="columnFlagPred">
        /// A caller-provided function that determines whether a particular row key should be
        /// included in the output based on the column flags of that row.
        /// </param>
        /// <returns>
        /// A dictionary containing the filtered subset of row keys and their column flags.
        /// </returns>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public Dictionary<int, uint> GetFilteredColumnFlags(Func<int, uint, bool> columnFlagPred)
        {
            var filtered = new Dictionary<int, uint>();
            foreach (var kvp in _tableColumnFlags)
            {
                int hashValue = kvp.Key;
                uint columnFlag = kvp.Value;
                if (!columnFlagPred(hashValue, columnFlag)) continue;
                filtered.Add(hashValue, columnFlag);
            }
            return filtered;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _ConfigureHashPointTable()
        {
            int columnCount = ImageKeys.Count;
            int capacity = _EstimateHashPointTableCapacity();
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_ConfigureHashPointTable)}"))
            {
                _table = new IntKeyIntRow(columnCount, capacity: capacity);
                _table.OnNewRow.Data = _FilledMinusOne();
                _tableColumnFlags = new Dictionary<int, uint>(capacity: capacity);
            }
        }

        private int[] _FilledMinusOne()
        {
            // New rows must be initialized to -1 to indicate it is not a valid index into the 
            // list of hash points for any image.
            int columnCount = ImageKeys.Count;
            int[] newRowData = new int[columnCount];
            for (int k = 0; k < columnCount; ++k)
            {
                newRowData[k] = -1;
            }
            return newRowData;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private int _EstimateHashPointTableCapacity()
        {
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_EstimateHashPointTableCapacity)}"))
            {
#if true
                int capacity = 0;
                foreach (int imageKey in ImageKeys)
                {
                    var hashPoints = HashPointSource[imageKey].HashPoints;
                    int pointCount = hashPoints.Count;
                    capacity += pointCount;
                }
                if (true)
                {
                    capacity = (int)Math.Round(capacity * 0.5);
                }
                if (true)
                {
                    int nextPow2 = 1;
                    while (nextPow2 < capacity)
                    {
                        nextPow2 *= 2;
                    }
                    capacity = nextPow2;
                }
                return capacity;
#else
            return 65536;
#endif
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _PopulateHashPointTable()
        {
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_PopulateHashPointTable)}()"))
            {
                foreach (int imageKey in ImageKeys)
                {
                    _PopulateHashPointTable(imageKey);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _PopulateHashPointTable(int imageKey)
        {
            int column = ImageKeys.IndexOf(imageKey);
            var hashPoints = HashPointSource[imageKey].HashPoints;
            int pointCount = hashPoints.Count;
            using (var timer = new MethodTimer($"{GetType().Name}.{nameof(_PopulateHashPointTable)}({imageKey})"))
            {
                for (int pointIndex = 0; pointIndex < pointCount; ++pointIndex)
                {
                    var hp = hashPoints[pointIndex];
                    int hashValue = hp.HashValue;
                    _table.WriteValue(hashValue, column, pointIndex);
                    _UpdateTableColumnFlag(hashValue, column);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void _UpdateTableColumnFlag(int hashValue, int column)
        {
            uint columnBit = (1u << column);
            if (_tableColumnFlags.TryGetValue(hashValue, out uint oldFlag))
            {
                _tableColumnFlags[hashValue] = oldFlag | columnBit;
            }
            else
            {
                _tableColumnFlags.Add(hashValue, columnBit);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _DiagDump()
        {
            var keys = _DiagDump_GetFilteredKeys();
            int keyCount = Math.Min(keys.Count, 100);
            int columnCount = _table.ColumnCount;
            string DataFunc(int row, int col)
            {
                return _table.TryGetValue(keys[row], col).Value.ToString();
            }
            string RowHeaderFunc(int row)
            {
                return keys[row].ToString("x8");
            }
            string ColumnHeaderFunc(int column)
            {
                int imageKey = ImageKeys[column];
                return $"img_{imageKey}";
            }
            ITextGridSource dataTGS = TextGridSource.Create(keyCount, columnCount, DataFunc);
            ITextGridSource headerTGS = TextGridSource.CreateWithHeaders(dataTGS, RowHeaderFunc, ColumnHeaderFunc);
            var tgf = new TextGridFormatter(headerTGS);
            //tgf.Output.ToConsole();
            //Debugger.Break();
        }

        private List<int> _DiagDump_GetFilteredKeys()
        {
            int columnCount = ImageKeys.Count;
            int minFoundCount = 3;
            int maxFoundCount = Math.Min(5, columnCount - 1);
            int countNonNegative(int[] rowData)
            {
                int nnCount = 0;
                for (int k = 0; k < rowData.Length; ++k)
                {
                    if (rowData[k] >= 0) ++nnCount;
                }
                return nnCount;
            }
            var filteredKeys = new List<int>();
            IntKeyIntRow.AfterVisit filterFunc(int hashValue, bool exists, int[] rowData)
            {
                int foundCount = countNonNegative(rowData);
                if (foundCount >= minFoundCount && foundCount <= maxFoundCount)
                {
                    filteredKeys.Add(hashValue);
                }
                return IntKeyIntRow.AfterVisit.NoChange;
            }
            _table.VisitAll(filterFunc);
            return filteredKeys;
        }

        private void _PairCoincidenceTable()
        { 
        }
    }
}
