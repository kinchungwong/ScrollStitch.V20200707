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

    public class FastRectNode<T>
        : IRectQuery<KeyValuePair<Rect, T>>
        , ICollection<KeyValuePair<Rect, T>>
        , IReadOnlyCollection<KeyValuePair<Rect, T>>
    {
        public Rect BoundingRect { get; }
        public FastRectNodeSettings Settings { get; }
        public bool HasComputedChildRects { get; private set; }
        public bool CanCreateChildNodes => (BoundingRect.Width >= 4 && BoundingRect.Height >= 4);
        public int Count { get; private set; }
        public bool IsReadOnly => false;

#region private
        private FastRectList _newRects;
        private List<T> _newData;
#endregion

#region private
        private FastRectList _childRects;
        private List<FastRectNode<T>> _childNodes;
#endregion

#region private
        private FastRectList _straddleRects;
        private List<T> _straddleData;
#endregion

        public FastRectNode(Rect boundingRect, FastRectNodeSettings settings)
        {
            BoundingRect = boundingRect;
            Settings = settings;
            HasComputedChildRects = false;
            _newRects = new FastRectList(boundingRect);
            _newData = new List<T>();
            _childRects = null;
            _childNodes = null;
            _straddleRects = null;
            _straddleData = null;
        }

        public void Clear()
        {
            _newRects.Clear();
            _newData.Clear();
            _childRects?.Clear();
            _childRects = null;
            _childNodes?.Clear();
            _childNodes = null;
            _straddleRects?.Clear();
            _straddleRects = null;
            _straddleData?.Clear();
            _straddleData = null;
            HasComputedChildRects = false;
            Count = 0;
            _CheckClassInvariantElseThrow();
        }

        public void Add(Rect rect, T data)
        {
            _CheckClassInvariantElseThrow();
            int index = _newRects.Add(rect);
            while (index >= _newData.Count)
            {
                _newData.Add(default);
            }
            _newData[index] = data;
            ++Count;
            if (_newRects.Count >= Settings.ProcessNewDataWhenCountReaches)
            {
                _ProcessNewData();
            }
            _CheckClassInvariantElseThrow();
        }

        public void Add(KeyValuePair<Rect, T> rectAndData)
        {
            Add(rectAndData.Key, rectAndData.Value);
        }

        public void AddRange(IEnumerable<KeyValuePair<Rect, T>> rectsAndData)
        {
            foreach (var kvp in rectsAndData)
            {
                Rect rect = kvp.Key;
                T data = kvp.Value;
                Add(rect, data);
            }
        }

        public void AddRange(ICollection<Rect> rectCollection, ICollection<T> dataCollection)
        {
            int count = rectCollection.Count;
            if (dataCollection.Count != count)
            {
                throw new ArgumentException("Count mismatch between the two collections.");
            }
            var rectEnumerator = rectCollection.GetEnumerator();
            var dataEnumerator = dataCollection.GetEnumerator();
            while (rectEnumerator.MoveNext() && dataEnumerator.MoveNext())
            {
                Rect rect = rectEnumerator.Current;
                T data = dataEnumerator.Current;
                Add(rect, data);
            }
        }

        public void AddRange(IReadOnlyCollection<Rect> rectCollection, IReadOnlyCollection<T> dataCollection)
        {
            int count = rectCollection.Count;
            if (dataCollection.Count != count)
            {
                throw new ArgumentException("Count mismatch between the two collections.");
            }
            var rectEnumerator = rectCollection.GetEnumerator();
            var dataEnumerator = dataCollection.GetEnumerator();
            while (rectEnumerator.MoveNext() && dataEnumerator.MoveNext())
            {
                Rect rect = rectEnumerator.Current;
                T data = dataEnumerator.Current;
                Add(rect, data);
            }
        }

        public void AddRange(IList<Rect> rectCollection, IList<T> dataCollection)
        {
            int count = rectCollection.Count;
            if (dataCollection.Count != count)
            {
                throw new ArgumentException("Count mismatch between the two collections.");
            }
            for (int index = 0; index < count; ++index)
            {
                Rect rect = rectCollection[index];
                T data = dataCollection[index];
                Add(rect, data);
            }
        }

        public void AddRange(IReadOnlyList<Rect> rectCollection, IReadOnlyList<T> dataCollection)
        {
            int count = rectCollection.Count;
            if (dataCollection.Count != count)
            {
                throw new ArgumentException("Count mismatch between the two collections.");
            }
            for (int index = 0; index < count; ++index)
            {
                Rect rect = rectCollection[index];
                T data = dataCollection[index];
                Add(rect, data);
            }
        }

        public IEnumerable<KeyValuePair<Rect, T>> Enumerate(Rect queryRect)
        {
            _CheckClassInvariantElseThrow();
            Rect? maybeModifiedQueryRect = InternalRectUtility.Inline.TryComputeIntersection(BoundingRect, queryRect);
            if (maybeModifiedQueryRect.HasValue)
            {
                queryRect = maybeModifiedQueryRect.Value;
                foreach (int newIndex in _newRects.Enumerate(queryRect))
                {
                    Rect itemRect = _newRects[newIndex];
                    T itemData = _newData[newIndex];
                    yield return new KeyValuePair<Rect, T>(itemRect, itemData);
                }
                if (!(_straddleRects is null))
                {
                    foreach (int straddleIndex in _straddleRects.Enumerate(queryRect))
                    {
                        Rect itemRect = _straddleRects[straddleIndex];
                        T itemData = _straddleData[straddleIndex];
                        yield return new KeyValuePair<Rect, T>(itemRect, itemData);
                    }
                }
                if (!(_childRects is null))
                {
                    foreach (int childIndex in _childRects.Enumerate(queryRect))
                    {
                        Rect childRect = _childRects[childIndex];
                        var childNode = _childNodes[childIndex];
                        if (childNode is null)
                        {
                            continue;
                        }
                        foreach (var kvp in childNode.Enumerate(queryRect))
                        {
                            yield return kvp;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Enumerates all items.
        /// </summary>
        /// <returns>
        /// An enumeration of all items.
        /// </returns>
        public IEnumerable<KeyValuePair<Rect, T>> Enumerate()
        {
            _CheckClassInvariantElseThrow();
            int newCount = _newRects.Count;
            for (int newIndex = 0; newIndex < newCount; ++newIndex)
            {
                Rect itemRect = _newRects[newIndex];
                T itemData = _newData[newIndex];
                yield return new KeyValuePair<Rect, T>(itemRect, itemData);
            }
            int straddleCount = _straddleRects?.Count ?? 0;
            for (int straddleIndex = 0; straddleIndex < straddleCount; ++straddleIndex)
            {
                Rect itemRect = _straddleRects[straddleIndex];
                T itemData = _straddleData[straddleIndex];
                yield return new KeyValuePair<Rect, T>(itemRect, itemData);
            }
            if (!(_childNodes is null))
            {
                foreach (var childNode in _childNodes)
                {
                    if (childNode is null)
                    {
                        continue;
                    }
                    foreach (var kvp in childNode.Enumerate())
                    {
                        yield return kvp;
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<Rect, T>> GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerate().GetEnumerator();
        }

        public bool Contains(KeyValuePair<Rect, T> queryRectAndData)
        {
            var defaultComparerForT = EqualityComparer<T>.Default;
            Rect queryRect = queryRectAndData.Key;
            foreach (var kvp in Enumerate(queryRect))
            {
                if (kvp.Key == queryRect &&
                    defaultComparerForT.Equals(queryRectAndData.Value, kvp.Value))
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

        /// <summary>
        /// <para>
        /// Not supported. <br/>
        /// <see cref="FastRectNode{T}"/> currently does not implement item removal.
        /// </para>
        /// </summary>
        /// <param name="item">
        /// Item to remove.
        /// </param>
        /// <returns></returns>
        /// 
        public bool Remove(KeyValuePair<Rect, T> item)
        {
            throw new NotImplementedException();
        }

        private void _ProcessNewData()
        {
            _EnsureStraddleListsCreated();
            _EnsureChildRectsComputed();
            if (!CanCreateChildNodes ||
                (_childRects?.Count ?? 0) == 0)
            {
                // Treats everything as straddle; clear up the new data queue.
                int requiredStraddleCapacity = _straddleRects.Count + _newRects.Count;
                if (_straddleRects.Capacity < requiredStraddleCapacity)
                {
                    _straddleRects.Capacity = requiredStraddleCapacity;
                    _straddleData.Capacity = requiredStraddleCapacity;
                }
                _straddleRects.AddRange(_newRects);
                _straddleData.AddRange(_newData);
                _newRects.Clear();
                _newData.Clear();
                return;
            }
            int newCount = _newRects.Count;
            for (int newIndex = 0; newIndex < newCount; ++newIndex)
            {
                // For each item rect, finds the first child node with a bounding rect 
                // that fully encompasses the item rect.
                //
                // This makes use of the fact that the list of child rects are sorted
                // in increasing size; i.e. the first found child will be the "snuggest"
                // for the given item rect.
                //
                // Refer to _PreallocateChildNodeList() for the arrangement of child 
                // bounding rectangles. It may be configurable via Settings.
                // 
                Rect itemRect = _newRects[newIndex];
                T itemData = _newData[newIndex];
                int childIndex = _childRects.FindFirstEncompassing(itemRect);
                if (childIndex >= 0)
                {
                    // Goes to a child.
                    //
                    var childNode = _EnsureChildNodeCreated(childIndex);
                    childNode.Add(itemRect, itemData);
                }
                else
                {
                    // Goes to straddle.
                    // 
                    _straddleRects.Add(itemRect);
                    _straddleData.Add(itemData);
                }
            }
            _newRects.Clear();
            _newData.Clear();
        }

        private void _EnsureStraddleListsCreated()
        {
            if (_straddleRects is null)
            {
                _straddleRects = new FastRectList(BoundingRect);
            }
            if (_straddleData is null)
            {
                _straddleData = new List<T>();
            }
        }

        private void _EnsureChildListsCreated()
        {
            if (_childRects is null)
            {
                _childRects = new FastRectList(BoundingRect);
            }
            if (_childNodes is null)
            {
                _childNodes = new List<FastRectNode<T>>();
            }
        }

        private FastRectNode<T> _EnsureChildNodeCreated(int childIndex)
        {
            var childNode = _childNodes[childIndex];
            if (childNode is null)
            {
                var childRect = _childRects[childIndex];
                childNode = new FastRectNode<T>(childRect, Settings);
                _childNodes[childIndex] = childNode;
            }
            return childNode;
        }

        private void _EnsureChildRectsComputed()
        {
            if (!CanCreateChildNodes)
            {
                return;
            }
            if (HasComputedChildRects)
            {
                // This function should only be executed once.
                return;
            }
            HasComputedChildRects = true;
            _EnsureChildListsCreated();
            _childRects.AddRange(Settings.ChildFactory.Enumerate(BoundingRect));
            while (_childNodes.Count < _childRects.Count)
            {
                _childNodes.Add(null);
            }
        }

        private void _CheckClassInvariantElseThrow()
        {
            string s = string.Empty;
            // ======
            // The new data list is always preallocated.
            // ======
            if (_newRects is null ||
                _newData is null ||
                _newRects.Count != _newData.Count)
            {
                s += "(_newRects, _newData); ";
            }
            // ======
            // Child rects are populated upon reaching the first ProcessNewData threshold.
            // ======
            int childRectCount = _childRects?.Count ?? 0;
            int childNodeCount = _childNodes?.Count ?? 0;
            if (childRectCount != childNodeCount)
            {
                s += "(_childRects, _childNodes); ";
            }
            // ======
            // Likewise, straddle list are populated upon reaching the first ProcessNewData threshold.
            // ======
            int straddleRectCount = _straddleRects?.Count ?? 0;
            int straddleDataCount = _straddleData?.Count ?? 0;
            if (straddleRectCount != straddleDataCount)
            {
                s += "(_straddleRects, _straddleData); ";
            }
            if (!string.IsNullOrEmpty(s))
            {
                throw new Exception("Class invariant violation: " + s);
            }
        }
    }
}
