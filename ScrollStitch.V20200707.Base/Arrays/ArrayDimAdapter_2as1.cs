using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ======
// TODO
// This class has not yet been checked for correctness or usefulness.
// ======

namespace ScrollStitch.V20200707.Arrays
{
    public class ArrayDimAdapter_2as1<T>
        : IArray<T>, IList<T>, IReadOnlyList<T>
    {
        private readonly IArray2<T> _array2;
        private readonly int _length0;
        private readonly int _length1;
        private readonly bool _transposed;

        public T this[int index]
        {
            get
            {
                (int idx0, int idx1) = _IndexToSubscript(index);
                return _array2[idx0, idx1];
            }
            set
            {
                (int idx0, int idx1) = _IndexToSubscript(index);
                _array2[idx0, idx1] = value;
            }
        }

        public T this[int idx0, int idx1]
        {
            get => _array2[idx0, idx1];
            set => _array2[idx0, idx1] = value;
        }

        public int Count => _length0 * _length1;

        public int Length => _length0 * _length1;

        public bool IsReadOnly => false;

        /// <inheritdoc cref="IList.IsFixedSize"/>
        public bool IsFixedSize => true;


        public ArrayDimAdapter_2as1(IArray2<T> array2, bool transposed)
        {
            _array2 = array2;
            _length0 = array2.GetLength(0);
            _length1 = array2.GetLength(1);
            _transposed = transposed;
        }

        public ArrayDimAdapter_2as1(T[,] array2, bool transposed)
            : this(new Array2Wrapper<T>(array2), transposed)
        {
        }

        /// <inheritdoc cref="IList{T}.IndexOf(T)"/>
        public int IndexOf(T item)
        {
            int count = Count;
            for (int k = 0; k < count; ++k)
            {
                if (this[k].Equals(item))
                {
                    return k;
                }
            }
            return -1;
        }

        /// <inheritdoc cref="IList{T}.Insert(int, T)"/>
        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IList{T}.RemoveAt(int)"/>
        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="ICollection{T}.Add(T)"/>
        public void Add(T item)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc cref="ICollection{T}.Clear()"/>
        public void Clear()
        {
            int count = Count;
            for (int k = 0; k < count; ++k)
            {
                this[k] = default;
            }
        }

        /// <inheritdoc cref="ICollection{T}.Contains(T)"/>
        public bool Contains(T item)
        {
            int count = Count;
            for (int k = 0; k < count; ++k)
            {
                if (this[k].Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc cref="ICollection{T}.CopyTo(T[], int)"/>
        public void CopyTo(T[] array, int arrayIndex)
        {
            for (int k = 0; k < Count; ++k)
            {
                array[arrayIndex + k] = this[k];
            }
        }

        public bool Remove(T item)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _Yield();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Yield();
        }

        private IEnumerator<T> _Yield()
        {
            for (int k = 0; k < Count; ++k)
            {
                yield return this[k];
            }
        }

        private (int idx0, int idx1) _IndexToSubscript(int linearIndex)
        {
            if (linearIndex < 0 ||
                linearIndex >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            if (_transposed)
            {
                return (linearIndex % _length0, linearIndex / _length0);
            }
            else
            {
                return (linearIndex / _length1, linearIndex % _length1);
            }
        }

        public int GetLength(int dim)
        {
            throw new NotImplementedException();
        }
    }
}
