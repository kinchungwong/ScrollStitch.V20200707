using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Functional;

    public static class FastRectDataList
    {
        public static class NoInline
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void _ThrowClassInvariantViolation()
            {
                throw new Exception("Class invariant violation.");
            }
        }
    }

    /// <summary>
    /// <see cref="FastRectDataList{T}"/> is a list of rectangles, each associated with a piece of data, and 
    /// provides support for <see cref="IRectQuery{T}"/>.
    /// 
    /// <para>
    /// Becuase this class is modeled after a list, it allows data records having the same rectangle to be 
    /// inserted multiple times.
    /// </para>
    /// 
    /// <para>
    /// This class requires a <see cref="BoundingRect"/> parameter. This class does not allow insertion 
    /// of rectangles that do not have positive width and height, or rectangles that are not fully 
    /// contained within the bounding rectangle of this instance.
    /// </para>
    /// 
    /// <para>
    /// Internally, this class relies on <see cref="FastRectList"/> for the rectangular query.
    /// </para>
    /// 
    /// </summary>
    /// <typeparam name="T">
    /// The type of data associated with each rectangle.
    /// </typeparam>
    /// 
    public class FastRectDataList<T>
        : IRectQuery<KeyValuePair<Rect, T>>
        , ICollection<KeyValuePair<Rect, T>>
        , IReadOnlyCollection<KeyValuePair<Rect, T>>
    {
        private FastRectList _rects;
        private List<T> _data;

        public Rect BoundingRect { get; }

        public int Count => _GetCount_Validated();

        public bool IsReadOnly => false;

        public FastRectDataList(Rect boundingRect)
            : this(boundingRect, capacity: 0)
        {
        }

        public FastRectDataList(Rect boundingRect, int capacity)
        {
            BoundingRect = boundingRect;
            _rects = new FastRectList(boundingRect, capacity: capacity);
            _data = new List<T>(capacity: capacity);
        }

        public void Add(Rect rect, T data)
        {
            int index = _rects.Add(rect);
            while (index >= _data.Count)
            {
                _data.Add(default);
            }
            _data[index] = data;
        }

        public void Add(KeyValuePair<Rect, T> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _rects.Clear();
            _data.Clear();
        }

        public bool Contains(KeyValuePair<Rect, T> item)
        {
            var dataComparer = EqualityComparer<T>.Default;
            foreach (int itemIndex in _rects.Enumerate(default(RectRelations.IdenticalNT), item.Key))
            {
                if (dataComparer.Equals(_data[itemIndex], item.Value))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(KeyValuePair<Rect, T>[] array, int arrayIndex)
        {
            if (array is null)
            {
                throw new ArgumentNullException(nameof(array));
            }
            if (arrayIndex < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            if (arrayIndex + Count > array.Length)
            {
                throw new ArgumentException(
                    message: $"The array section starting at (arrayIndex = {arrayIndex}) ending at " +
                    $"(array.Length = {array.Length}) " +
                    $"does not provide sufficient space to copy all items (count = {Count}).");
            }
            foreach (var kvp in Enumerate())
            {
                array[arrayIndex++] = kvp;
            }
        }

        public IEnumerable<KeyValuePair<Rect, T>> Enumerate()
        {
            int count = _GetCount_Validated();
            for (int index = 0; index < count; ++index)
            {
                yield return new KeyValuePair<Rect, T>(_rects[index], _data[index]);
            }
        }

        public IEnumerable<KeyValuePair<Rect, T>> Enumerate(Rect queryRect)
        {
            foreach (int itemIndex in _rects.Enumerate(queryRect))
            {
                yield return new KeyValuePair<Rect, T>(_rects[itemIndex], _data[itemIndex]);
            }
        }

        public IEnumerator<KeyValuePair<Rect, T>> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        public bool Remove(KeyValuePair<Rect, T> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int _GetCount_Validated()
        {
            if (_rects is null ||
                _data is null)
            {
                FastRectDataList.NoInline._ThrowClassInvariantViolation();
            }
            int count = _rects.Count;
            int dataCount = _data.Count;
            if (count != dataCount)
            {
                FastRectDataList.NoInline._ThrowClassInvariantViolation();
            }
            return count;
        }
    }
}
