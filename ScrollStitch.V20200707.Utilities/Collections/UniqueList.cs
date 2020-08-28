using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections
{
    /// <summary>
    /// <para>
    /// A list for unique items.
    /// </para>
    /// <para>
    /// The list maintains the order of items in the same way as they are added. Each new item is 
    /// sequentially assigned an index value, which is the same value as if the new item is added 
    /// to an ordinary <see cref="System.Collections.Generic.List{T}"/>.
    /// </para>
    /// <para>
    /// Implementation detail. This class relies on a <see cref="Dictionary{T, int}}"/> for its 
    /// content-based lookup functionality. The dictionary, in turn, depends on <see cref="T"/> 
    /// having a meaningful implementation of <see cref="Object.GetHashCode()"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="T">
    /// The type of unique items that can be added to this class.
    /// </typeparam>
    public class UniqueList<T>
        : IUniqueList<T>
        , IReadOnlyList<T>
    {
        #region private
        private readonly List<T> _items;
        private readonly Dictionary<T, int> _lookup;
        #endregion

        #region public properties
        /// <summary>
        /// The number of unique items on the list.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// <para>
        /// Looks up the sequentially assigned index of the specified item. If the item is new,
        /// it is automatically added to the list and assigned the next available index.
        /// </para>
        /// <para>
        /// Same as <see cref="UniqueList{T}.IndexOf(T)"/>.
        /// </para>
        /// <para>
        /// Remark: This class provides two indexers. If the indexers are unavailable due to ambiguity,
        /// resolve the problem by calling <see cref="UniqueList{T}.IndexOf(T)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public int this[T t] => IndexOf(t);

        /// <summary>
        /// <para>
        /// Returns the item at the specified index.
        /// </para>
        /// <para>
        /// Same as <see cref="UniqueList{T}.ItemAt(int)"/>.
        /// </para>
        /// <para>
        /// Remark: This class provides two indexers. If the indexers are unavailable due to ambiguity,
        /// resolve the problem by calling <see cref="UniqueList{T}.ItemAt(int)"/> instead.
        /// </para>
        /// </summary>
        /// <param name="index">The index of the item to be looked up.</param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">
        /// </exception>
        public T this[int index] => ItemAt(index);
        #endregion

        #region constructors
        /// <summary>
        /// 
        /// </summary>
        public UniqueList()
        {
            _items = new List<T>();
            _lookup = new Dictionary<T, int>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public UniqueList(IEnumerable<T> items)
            : this()
        {
            AddRange(items);
        }
        #endregion

        /// <summary>
        /// <para>
        /// Returns the index for the given item, without adding the item to the list.
        /// </para>
        /// <para>
        /// See also: <see cref="UniqueList{T}.Add(T)"/>, which automatically adds the 
        /// item to the list if it is new.
        /// </para>
        /// </summary>
        /// <param name="t">
        /// The item to look up.
        /// </param>
        /// <returns>
        /// The index of the item, or -1 if the item has never been added before.
        /// </returns>
        public int IndexOf(T t)
        {
            if (_lookup.TryGetValue(t, out int index))
            {
                return index;
            }
            return -1;
        }

        /// <summary>
        /// <para>
        /// Adds the item to the list, but only if it has not been added before. The assigned index is returned.
        /// </para>
        /// <para>
        /// See also: <see cref="UniqueList{T}.IndexOf(T)"/>, which does not automatically add
        /// any new item.
        /// </para>
        /// </summary>
        /// <param name="t">
        /// The item to be added (if new) or looked up (if existing).
        /// </param>
        /// <returns>
        /// The sequentially assigned index for the item on the list.
        /// </returns>
        public int Add(T t)
        {
            if (_lookup.TryGetValue(t, out int index))
            {
                return index;
            }
            _items.Add(t);
            index = _items.Count - 1;
            _lookup.Add(t, index);
            return index;
        }

        /// <summary>
        /// <para>
        /// Returns the item stored at the specified index.
        /// </para>
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException">
        /// There is no item stored at the specified index.
        /// </exception>
        public T ItemAt(int index)
        {
            return _items[index];
        }

        /// <summary>
        /// Adds or looks up multiple items. Any item that does not already exist on the UniqueList
        /// are automatically added.
        /// </summary>
        /// <param name="items">List of items to be added or looked up.</param>
        /// <returns>
        /// A list of integer index corresponding to the sequentially assigned index for each input item.
        /// </returns>
        public List<int> AddRange(IEnumerable<T> items)
        {
            var results = new List<int>();
            foreach (var t in items)
            {
                results.Add(Add(t));
            }
            return results;
        }

        /// <summary>
        /// Returns a copy of the list of items.
        /// </summary>
        /// <returns>
        /// A copy of the list. Operations on the returned list does not interfere with the UniqueList instance.
        /// </returns>
        public List<T> ToList()
        {
            return new List<T>(_items);
        }

        /// <summary>
        /// Returns an array containing the same items as UniqueList.
        /// </summary>
        /// <returns>
        /// An array containing the items. Operations on the returned does not interfere with the UniqueList instance.
        /// </returns>
        public T[] ToArray()
        {
            return _items.ToArray();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)_items).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
