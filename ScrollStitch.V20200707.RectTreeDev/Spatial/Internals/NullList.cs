using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    /// <summary>
    /// <see cref="NullList{T}"/> is a fake list that doesn't store anything.
    /// 
    /// <para>
    /// This class is typically used in composite associative data structures 
    /// (with an internal implementation of SoA (structure-of-arrays)) in which:
    /// <br/>
    /// ... The user of that data structure doesn't need to use one or more dependent fields,
    /// <br/>
    /// ... There is no concrete implementation that efficiently skips over those unneeded fields.
    /// </para>
    /// 
    /// <para>
    /// For these users, a convenient way to cut down on those storage is to instruct 
    /// the composite associative data structure to store the unneeded fields inside an 
    /// instnace of this class. Items added to this class are simply ignored.
    /// </para>
    /// 
    /// <para>
    /// This class maintains the <see cref="Count"/>, <see cref="Capacity"/>, and a few other 
    /// properties so that it will behave superficially like a class that is seemingly capable
    /// of storing default values of the item type.
    /// </para>
    /// 
    /// <para>
    /// Many methods that require validated index or count arguments by contract will still be
    /// performing those validations. This means exceptions could be thrown.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// Nominally, the item type that is handled by this class. 
    /// <br/>
    /// In practice, any type that can be defaulted (at zero runtime cost) can be used.
    /// </typeparam>
    /// 
    public class NullList<T>
        : IList<T>
        , IReadOnlyList<T>
#if false
        , ICollection<T> /*redundant*/
        , IEnumerable<T> /*redundant*/
        , IReadOnlyCollection<T> /*redundant*/
#endif
    {
        public bool IsReadOnly => false;

        public int Count 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set; 
        }

        public int Capacity 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Count;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value < Count)
                {
                    throw new ArgumentOutOfRangeException("Capacity is set to a value that is less than Count.");
                }
                // Otherwise the value is discarded.
            }
        }

        public T this[int index] 
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => default;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (unchecked((uint)index >= (uint)Count))
                {
                    throw new IndexOutOfRangeException();
                }
                // else value is discarded.
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NullList()
        { 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NullList(int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NullList(int capacity, int count)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }
            if (count < 0 || count > capacity)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            Count = count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T _)
        {
            ++Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddRange(IEnumerable<T> itemsOfNull)
        {
            switch (itemsOfNull)
            {
                case ICollection<T> icoll:
                    Count += icoll.Count;
                    return;
                case IReadOnlyCollection<T> irocoll:
                    Count += irocoll.Count;
                    return;
            }
            foreach (var _ in itemsOfNull)
            {
                ++Count;
            }
            return;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Count = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(T _)
        {
            return Count > 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void CopyTo(T[] array, int arrayIndex)
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
            Arrays.BuiltinArrayMethods.NoInline.ArrayFill(array, default, arrayIndex, Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerator<T> GetEnumerator()
        {
            int count = Count;
            for (int k = 0; k < count; ++k)
            {
                yield return default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int IndexOf(T _)
        {
            // Since every element is "default", 
            // the first match is the first item, at index zero.
            // Except if the list is empty.
            return (Count == 0) ? -1 : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int LastIndexOf(T _)
        {
            // Since every element is "default", 
            // the last match is the last item, at index (Count - 1).
            // Except if the list is empty.
            return (Count == 0) ? -1 : (Count - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, T _)
        {
            // Note: argument index == Count is valid.
            if (index < 0 ||
                index > Count)
            {
                throw new ArgumentOutOfRangeException();
            }
            ++Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Remove(T _)
        {
            if (Count > 0)
            {
                --Count;
                return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            if (unchecked((uint)index >= (uint)Count))
            {
                throw new IndexOutOfRangeException();
            }
            --Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort()
        { 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(Comparison<T> _)
        { 
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(IComparer<T> _)
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Sort(int index, int count, IComparer<T> comparer)
        {
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
