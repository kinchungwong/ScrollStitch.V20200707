using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals.RectTreeV1
{
    using ScrollStitch.V20200707.Data;

    public class Node
    {
        public NodeBounds Bounds { get; }

        public NodeSettings Settings { get; }

        public List<Record>[] RecordList { get; }

        public Node[] ChildList { get; }

        public bool CanCreateChildNodes { get; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public Node(NodeBounds bounds, NodeSettings settings)
        {
            Bounds = bounds;
            Settings = settings;
            RecordList = new List<Record>[5];
            ChildList = new Node[5];
            int halfBoundLength = Math.Min(Bounds.HalfSize.Width, Bounds.HalfSize.Height);
            CanCreateChildNodes = halfBoundLength >= NodeBounds.AllowedMinRectLength;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(Rect rect, int index)
        {
            ItemFlag flag = Bounds.ClassifyItem(rect);
            WhichChild whichChild = flag.FlagToChild();
            _InternalAdd(whichChild, rect, index, flag);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        /*[RuntimeMaxRecursionDepth(32)]*/
        private void _InternalAdd(WhichChild whichChild, Rect rect, int index, ItemFlag flag)
        {
            int childIndex = (int)whichChild;
            if (childIndex < 0 || childIndex >= 5)
            {
                _Throw();
            }
            Node childNode = ChildList[childIndex];
            if (!(childNode is null))
            {
                childNode.Add(rect, index);
                return;
            }
            List<Record> list = RecordList[childIndex];
            if (list is null)
            {
                list = new List<Record>(capacity: Settings?.ListInitialCapacity ?? 1);
                RecordList[childIndex] = list;
            }
            list.Add(new Record(rect, index, flag));
            _InternalTrySplit(whichChild);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _InternalTrySplit(WhichChild whichChild)
        {
            if (!CanCreateChildNodes ||
                !whichChild.IsSplittable() ||
                Settings is null)
            {
                return;
            }
            int childIndex = (int)whichChild;
            if (childIndex < 0 || childIndex >= 5)
            {
                _Throw();
            }
            List<Record> list = RecordList[childIndex];
            if (list is null)
            {
                return;
            }
            if (list.Count >= Settings.EachListToNodeThreshold)
            {
                _InternalConvertListToNode(whichChild);
                return;
            }
            int totalListCount =
                (RecordList[0]?.Count ?? 0) +
                (RecordList[1]?.Count ?? 0) +
                (RecordList[2]?.Count ?? 0) +
                (RecordList[3]?.Count ?? 0);
            if (totalListCount >= Settings.TotalListToNodeThreshold)
            {
                _InternalSplitAll();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _InternalSplitAll()
        {
            for (int childIndex = 0; childIndex < 4; ++childIndex)
            {
                if (!(RecordList[childIndex] is null) &&
                    ChildList[childIndex] is null)
                {
                    WhichChild whichChild = (WhichChild)childIndex;
                    _InternalConvertListToNode(whichChild);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void _InternalConvertListToNode(WhichChild whichChild)
        {
            int childIndex = (int)whichChild;
            if (childIndex < 0 || childIndex >= 4)
            {
                _Throw();
            }
            if (!CanCreateChildNodes)
            {
                _Throw();
            }
            if (RecordList[childIndex] is null ||
                !(ChildList[childIndex] is null))
            {
                _Throw();
            }
            Bounds.Deconstruct(out int left, out int center, out int right, out int top, out int middle, out int bottom);
            int x1, x2, y1, y2;
            switch (whichChild)
            {
                case WhichChild.TopLeft:
                    x1 = left;
                    x2 = center;
                    y1 = top;
                    y2 = middle;
                    break;
                case WhichChild.TopRight:
                    x1 = center;
                    x2 = right;
                    y1 = top;
                    y2 = middle;
                    break;
                case WhichChild.BottomLeft:
                    x1 = left;
                    x2 = center;
                    y1 = middle;
                    y2 = bottom;
                    break;
                case WhichChild.BottomRight:
                    x1 = center;
                    x2 = right;
                    y1 = middle;
                    y2 = bottom;
                    break;
                default:
                    throw new Exception();
            }
            Rect childRect = new Rect(x1, y1, x2 - x1, y2 - y1);
            var childNode = new Node(new NodeBounds(childRect), Settings);
            ChildList[childIndex] = childNode;
            foreach (var record in RecordList[childIndex])
            {
                childNode.Add(record.Rect, record.Index);
            }
            RecordList[childIndex] = null;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        /*[RuntimeMaxRecursionDepth(32)]*/
        public void Query(Rect queryRect, Action<Rect, int> resultFunc)
        {
            ItemFlag queryMask = Bounds.ClassifyItem(queryRect);
            for (int childIndex = 0; childIndex < 5; ++childIndex)
            {
                WhichChild whichChild = (WhichChild)childIndex; // always valid
                ItemFlag childMask = whichChild.ChildToFlag();
                if ((queryMask & childMask) != ItemFlag.None)
                {
                    var records = RecordList[childIndex];
                    var child = ChildList[childIndex];
                    if (!(records is null))
                    {
                        _InternalQueryInRecordList(records, queryRect, resultFunc);
                    }
                    if (!(child is null))
                    {
                        child.Query(queryRect, resultFunc);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _InternalQueryInRecordList(List<Record> records, Rect queryRect, Action<Rect, int> resultFunc)
        {
            foreach (var record in records)
            {
                if (_InternalHasIntersect(record.Rect, queryRect))
                {
                    resultFunc(record.Rect, record.Index);
                }
            }
        }

        /// <summary>
        /// Internal implementation of checking existence of intersection between two rectangles.
        /// 
        /// <para>
        /// RectTree requires its own implementation for the following reasons:
        /// <br/>
        /// Firstly, RectTree requires an implementation that validates both rectangle arguments 
        /// to have positive width and height. 
        /// <br/>
        /// Also, a result of true (the existence of intersection) requires the intersection width 
        /// and height to be positive.
        /// </para>
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool _InternalHasIntersect(Rect a, Rect b)
        {
            if (a.Width <= 0 || a.Height <= 0 ||
                b.Width <= 0 || b.Height <= 0)
            {
                return _Throw<bool>();
            }
            int maxLeft = Math.Max(a.Left, b.Left);
            int minRight = Math.Min(a.Right, b.Right);
            int maxTop = Math.Max(a.Top, b.Top);
            int minBottom = Math.Min(a.Bottom, b.Bottom);
            return (maxLeft < minRight) && (maxTop < minBottom);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static PseudoReturnType _Throw<PseudoReturnType>()
        {
            throw new Exception();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void _Throw()
        {
            throw new Exception();
        }
    }
}
