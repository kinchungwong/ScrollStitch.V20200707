using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using Data;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// An implementation of <see cref="IRectTree{RecordType}"/> 
    /// </summary>
    /// <typeparam name="RecordType"></typeparam>
    public class RectList<RecordType>
        : IRectTree<RecordType>
    {
        #region
        private List<RecordType> _records;
        #endregion

        public RectList(Func<RecordType, Rect> recordToRectFunc)
        {
            RecordToRectFunc = recordToRectFunc;
            _records = new List<RecordType>();
        }

        public RectList(Func<RecordType, Rect> recordToRectFunc, int capacity)
        {
            RecordToRectFunc = recordToRectFunc;
            _records = new List<RecordType>(capacity: capacity);
        }

        /// <inheritdoc/>
        public Type GetRecordType()
        {
            return typeof(RecordType);
        }

        /// <inheritdoc/>
        public Func<RecordType, Rect> RecordToRectFunc
        {
            get;
            set;
        }

        /// <inheritdoc/>
        public IEqualityComparer<RecordType> RecordEqualityFunc
        {
            get;
            set;
        } = EqualityComparer<RecordType>.Default;

        /// <inheritdoc/>
        public bool Contains(RecordType record)
        {
            return _records.Contains(record);
        }

        /// <inheritdoc/>
        public void Add(RecordType record)
        {
            _records.Add(record);
        }

        /// <inheritdoc/>
        public void AddRange(IEnumerable<RecordType> records)
        {
            foreach (var record in records)
            {
                Add(record);
            }
        }

        /// <inheritdoc/>
        public void Remove(RecordType record)
        {
            _records.Remove(record);
        }

        /// <inheritdoc/>
        public void RemoveRange(IEnumerable<RecordType> records)
        {
            foreach (var record in records)
            {
                Remove(record);
            }
        }

        /// <inheritdoc/>
        public void ForEach(Action<RecordType> action)
        {
            foreach (var record in _records)
            {
                action(record);
            }
        }

        /// <inheritdoc/>
        public void ForEach(Rect searchRect, Action<RecordType> action)
        {
            foreach (var record in _records)
            {
                if (!RectTreeUtility.HasPositiveOverlap(searchRect, RecordToRectFunc(record)))
                {
                    continue;
                }
                action(record);
            }
        }

        /// <inheritdoc/>
        public RecordType[] ToArray()
        {
            return _records.ToArray();
        }

        /// <inheritdoc/>
        public RecordType[] ToArray(Rect searchRect)
        {
            var list = new List<RecordType>();
            ForEach(searchRect, (RecordType record) => list.Add(record));
            return list.ToArray();
        }

        /// <inheritdoc/>
        public IEnumerable<RecordType> Enumerate()
        {
            foreach (var record in _records)
            {
                yield return record;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<RecordType> Enumerate(Rect searchRect)
        {
            foreach (var record in _records)
            {
                if (!RectTreeUtility.HasPositiveOverlap(searchRect, RecordToRectFunc(record)))
                {
                    continue;
                }
                yield return record;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            _records.Clear();
        }

        public IEnumerator<RecordType> GetEnumerator()
        {
            return _records.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _records.GetEnumerator();
        }
    }
}
