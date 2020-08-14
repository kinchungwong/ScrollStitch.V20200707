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
    {
        public Rect BoundingRect { get; }
        public FastRectNodeSettings Settings { get; }
        public bool CanCreateChildNodes => (BoundingRect.Width >= 4 && BoundingRect.Height >= 4);

        #region private
        private readonly FastRectList _newRects;
        private readonly List<T> _newData;
        #endregion

        #region private
        private readonly FastRectList _childRects;
        private readonly List<FastRectNode<T>> _childNodes;
        #endregion

        #region private
        private readonly FastRectList _straddleRects;
        private readonly List<T> _straddleData;
        #endregion

        public FastRectNode(Rect boundingRect, FastRectNodeSettings settings)
        {
            BoundingRect = boundingRect;
            Settings = settings;
            _newRects = new FastRectList(boundingRect);
            _newData = new List<T>();
            _childRects = new FastRectList(boundingRect);
            _childNodes = new List<FastRectNode<T>>();
            _straddleRects = new FastRectList(boundingRect);
            _straddleData = new List<T>();
            _PreallocateChildNodeList();
        }

        public void Clear()
        {
            _newRects.Clear();
            _newData.Clear();
            _childRects.Clear();
            _childNodes.Clear();
            _straddleRects.Clear();
            _straddleData.Clear();
            _PreallocateChildNodeList();
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
                foreach (int straddleIndex in _straddleRects.Enumerate(queryRect))
                {
                    Rect itemRect = _straddleRects[straddleIndex];
                    T itemData = _straddleData[straddleIndex];
                    yield return new KeyValuePair<Rect, T>(itemRect, itemData);
                }
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

        private void _ProcessNewData()
        {
            if (!CanCreateChildNodes)
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
                    var childRect = _childRects[childIndex];
                    var childNode = _childNodes[childIndex];
                    if (childNode is null)
                    {
                        childNode = new FastRectNode<T>(childRect, Settings);
                        _childNodes[childIndex] = childNode;
                    }
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

        private void _PreallocateChildNodeList()
        {
            if (!CanCreateChildNodes)
            {
                return;
            }
            // ====== Developer advice ======
            // Use graphical child node visualizer to compare between these configurations.
            // ======
#if true
            if (BoundingRect.Width >= 2 && BoundingRect.Height >= 2)
            {
                _PreallocateChildNodeList(axisCellCount: 2, cellHorzCount: 1, cellVertCount: 1);
                return;
            }
#elif false
            if (BoundingRect.Width >= 4 && BoundingRect.Height >= 4)
            {
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 1, cellVertCount: 1);
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 1, cellVertCount: 2);
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 2, cellVertCount: 1);
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 2, cellVertCount: 2);
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 1, cellVertCount: 4);
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 4, cellVertCount: 1);
                return;
            }
#elif false
            if (BoundingRect.Width >= 8 && BoundingRect.Height >= 8)
            {
                _PreallocateChildNodeList(axisCellCount: 8, cellHorzCount: 1, cellVertCount: 1);
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 2, cellVertCount: 2);
                return;
            }
#endif
        }

        private void _PreallocateChildNodeList(int axisCellCount, int cellHorzCount, int cellVertCount)
        {
            if (axisCellCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(axisCellCount));
            }
            if (cellHorzCount <= 0 || cellHorzCount > axisCellCount)
            {
                throw new ArgumentOutOfRangeException(nameof(cellHorzCount));
            }
            if (cellVertCount <= 0 || cellVertCount > axisCellCount)
            {
                throw new ArgumentOutOfRangeException(nameof(cellVertCount));
            }
            for (int cellY = 0; cellY + cellVertCount <= axisCellCount; ++cellY)
            {
                for (int cellX = 0; cellX + cellHorzCount <= axisCellCount; ++cellX)
                {
                    _PreallocateChildNodeList(axisCellCount, cellHorzCount, cellVertCount, cellX, cellY);
                }
            }
        }

        private void _PreallocateChildNodeList(int axisCellCount, int cellHorzCount, int cellVertCount, int cellX, int cellY)
        {
            if (cellX < 0 || cellX + cellHorzCount > axisCellCount)
            {
                throw new ArgumentOutOfRangeException(nameof(cellX));
            }
            if (cellY < 0 || cellY + cellVertCount > axisCellCount)
            {
                throw new ArgumentOutOfRangeException(nameof(cellX));
            }
            int boundX = BoundingRect.X;
            int boundY = BoundingRect.Y;
            int boundWidth = BoundingRect.Width;
            int boundHeight = BoundingRect.Height;
            int startX = boundX + (boundWidth * cellX) / axisCellCount;
            int stopX = boundX + (boundWidth * (cellX + cellHorzCount)) / axisCellCount;
            int startY = boundY + (boundHeight * cellY) / axisCellCount;
            int stopY = boundY + (boundHeight * (cellY + cellVertCount)) / axisCellCount;
            var childRect = new Rect(startX, startY, stopX - startX, stopY - startY);
            int childIndex = _childRects.Add(childRect);
            while (childIndex >= _childNodes.Count)
            {
                _childNodes.Add(null);
            }
            _childNodes[childIndex] = new FastRectNode<T>(childRect, Settings);
        }

        private void _CheckClassInvariantElseThrow()
        {
            string s = string.Empty;
            if (_newRects is null ||
                _newData is null ||
                _newRects.Count != _newData.Count)
            {
                s += "(_newRects, _newData); ";
            }
            if (_childRects is null ||
                _childNodes is null ||
                _childRects.Count != _childNodes.Count)
            {
                s += "(_childRects, _childNodes); ";
            }
            if (_straddleRects is null ||
                _straddleData is null ||
                _straddleRects.Count != _straddleData.Count)
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
