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
        public bool HasComputedChildRects { get; private set; }
        public bool CanCreateChildNodes => (BoundingRect.Width >= 4 && BoundingRect.Height >= 4);

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
            // ====== Developer advice ======
            // Use graphical child node visualizer to compare between these configurations.
            // ======
            bool canInsert2x2 = (BoundingRect.Width >= 2 && BoundingRect.Height >= 2);
            bool canInsert4x4 = (BoundingRect.Width >= 4 && BoundingRect.Height >= 4);
            bool canInsert5x5 = (BoundingRect.Width >= 5 && BoundingRect.Height >= 5);
            bool canInsert7x7 = (BoundingRect.Width >= 7 && BoundingRect.Height >= 7);
            bool canInsert8x8 = (BoundingRect.Width >= 8 && BoundingRect.Height >= 8);
            //
            // ====== Order of child rectangle insertion ======
            // During item processing in _ProcessNewData(), each item is sent to the first 
            // child node on the list whose bounding rectangle encompasses the item's rectangle.
            //
            // Thus, to maximize specificity (query efficiency), the most specific (tiniest)
            // rectangles should be inserted upfront so that they are preferentially selected.
            // ======
            //
            _EnsureChildListsCreated();
            if (Settings.ChildGrid_8x8_1x1 && canInsert8x8)
            {
                _PreallocateChildNodeList(axisCellCount: 8, cellHorzCount: 1, cellVertCount: 1);
            }
            if (Settings.ChildGrid_5x5_1x1 && canInsert5x5)
            {
                _PreallocateChildNodeList(axisCellCount: 5, cellHorzCount: 1, cellVertCount: 1);
            }
            if (Settings.ChildGrid_4x4_1x1 && canInsert4x4)
            {
                // Small (1x1 inside a grid of 4x4)
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 1, cellVertCount: 1);
            }
            if (Settings.ChildGrid_8x8_2x2 && canInsert8x8)
            {
                _PreallocateChildNodeList(axisCellCount: 8, cellHorzCount: 2, cellVertCount: 2);
            }
            if (Settings.ChildGrid_5x5_2x2 && canInsert5x5)
            {
                _PreallocateChildNodeList(axisCellCount: 5, cellHorzCount: 2, cellVertCount: 2);
            }
            if (Settings.ChildGrid_4x4_1x2 && canInsert4x4)
            {
                // Straddles (1x2 or 2x1 inside a grid of 4x4, overlapping)
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 1, cellVertCount: 2);
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 2, cellVertCount: 1);
            }
            if (Settings.ChildGrid_8x8_3x3 && canInsert8x8)
            {
                _PreallocateChildNodeList(axisCellCount: 8, cellHorzCount: 3, cellVertCount: 3);
            }
            if (Settings.ChildGrid_7x7_3x3 && canInsert7x7)
            {
                // This one requires special handling.
                for (int cellY = 0; cellY <= 4; cellY += 2)
                {
                    for (int cellX = 0; cellX <= 4; cellX += 2)
                    {
                        _PreallocateChildNodeList(axisCellCount: 7, cellHorzCount: 3, cellVertCount: 3, cellX: cellX, cellY: cellY);
                    }
                }
            }
            if (Settings.ChildGrid_5x5_3x3 && canInsert5x5)
            {
                // This one requires special handling.
                for (int cellY = 0; cellY <= 2; cellY += 2)
                {
                    for (int cellX = 0; cellX <= 2; cellX += 2)
                    {
                        _PreallocateChildNodeList(axisCellCount: 5, cellHorzCount: 3, cellVertCount: 3, cellX: cellX, cellY: cellY);
                    }
                }
            }
            if (Settings.ChildGrid_2x2_1x1 && canInsert2x2)
            {
                // Mimics a traditional quadtree, with 4 children, one for each quadrant.
                // Not recommended - performance is not as good.
                _PreallocateChildNodeList(axisCellCount: 2, cellHorzCount: 1, cellVertCount: 1);
            }
            if (Settings.ChildGrid_4x4_2x2 && canInsert4x4)
            {
                // Medium (2x2 inside a grid of 4x4, overlapping)
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 2, cellVertCount: 2);
            }
            if (Settings.ChildGrid_4x4_3x3 && canInsert4x4)
            {
                // Somewhat big (3x3 inside a grid of 4x4, overlapping)
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 3, cellVertCount: 3);
            }
            if (Settings.ChildGrid_4x4_1x4 && canInsert4x4)
            {
                // High aspect ratio (1:4)
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 1, cellVertCount: 4);
                _PreallocateChildNodeList(axisCellCount: 4, cellHorzCount: 4, cellVertCount: 1);
            }
            _CheckChildRectsUniqueElseThrow();
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
            // ====== Class invariant and runtime behavior ======
            // Class invariant requires the child rect list and child node list to have same Count.
            //
            // However, the elements in the child node list shall be null, initially, to prevent
            // egregious upfront allocation which can consume resources explosively.
            //
            // Actual child node instantiation is handled by _EnsureChildNodeCreated(childIndex).
            // ======
            int childIndex = _childRects.Add(childRect);
            while (childIndex >= _childNodes.Count)
            {
                _childNodes.Add(null);
            }
        }

        private void _CheckChildRectsUniqueElseThrow()
        {
            var hashedChildRects = new HashSet<Rect>(capacity: _childRects.Count);
            foreach (var childRect in _childRects)
            {
                if (hashedChildRects.Contains(childRect))
                {
                    throw new Exception("Class invariant violation: all computed child rects must be unique.");
                }
                hashedChildRects.Add(childRect);
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
