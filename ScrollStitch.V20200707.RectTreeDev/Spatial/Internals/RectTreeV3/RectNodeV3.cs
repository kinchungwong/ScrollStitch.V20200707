using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ====== IN PROGRESS ======
// Implementation paused (2020-08-27) due to changed approach
// ======

#if false
namespace ScrollStitch.V20200707.Spatial.Internals.RectTreeV3
{
    using ScrollStitch.V20200707.Data;

    public class RectNodeSettingsV3
    {
        public static RectNodeSettingsV3 DefaultInstance { get; } = new RectNodeSettingsV3();

        public int SplitThreshold { get; set; } = 100;

        public int HorzDiv { get; set; } = 2;

        public int VertDiv { get; set; } = 2;

        public int HighAspectRatioCriteria { get; set; } = 4;
    }

    public class RectNodeV3<T>
    {
        public Rect BoundingRect { get; }

        public RectNodeSettingsV3 Settings { get; }

        public bool CanSplit { get; private set; }

        public bool HasSplit => CanSplit && !(_children is null);

        public int SelfCount => Math.Min(Math.Min(
            _rects?.Count ?? 0, _masks?.Count ?? 0), _data?.Count ?? 0);

        public List<Rect> _rects;
        public List<RectMask128> _masks;
        public IList<T> _data;

#if true
        /// <summary>
        /// Number of items (among those considered "self") which have high aspect ratio.
        /// </summary>
        public int SelfHighAspectRatioCount { get; private set; }
#endif

#if true
        /// <summary>
        /// Number of items (among those considered "self") that cannot be split anyway,
        /// because they straddle in both dimensions (both horizontally and vertically).
        /// </summary>
        public int SelfStraddleCount { get; private set; }
#endif

        public List<RectNodeV3<T>> _children;

        private static class SpecialChildIndex
        {
            /// <summary>
            /// The item cannot have a child index computed because the node to which
            /// it is added is not capable of splitting. See CanSplit property.
            /// </summary>
            internal const int ChildlessNode = -1;

            /// <summary>
            /// The item straddles between two or more children bounding rectangles.
            /// More specifically, there is not a single child bounding rectangle that
            /// encompasses the item, thus the item cannot be added to any one of the 
            /// children nodes.
            /// </summary>
            internal const int Straddle = -2;

            /// <summary>
            /// Same general reason as Straddle, but with the additional observation that
            /// the item has a high aspect ratio, meaning that there is an unexploited
            /// opportunity to optimize - to potentially exclude clusters of items not 
            /// intersecting with a particular query.
            /// 
            /// If high aspect ration handling is not enabled, such items are treated
            /// the same way as Straddle.
            /// </summary>
            internal const int HighAspectRatio = -3;
        }

        public RectNodeV3(Rect boundingRect, RectNodeSettingsV3 settings)
        {
            BoundingRect = boundingRect;
            Settings = settings;
        }

        public RectNodeV3(Rect boundingRect)
            : this(boundingRect, RectNodeSettingsV3.DefaultInstance)
        { 
        }

#if true
        public void Add(Rect itemRect, T itemData)
        {
            RectNodeV3<T> target = _FindAddTarget(itemRect);
            target._AddToSelf(itemRect, itemData);
            target._TrySplit();
        }

        private RectNodeV3<T> _FindAddTarget(Rect itemRect)
        {
            RectNodeV3<T> target = this;
            while (target.HasSplit)
            {
                int childIndex = target._TryGetChildIndex(itemRect);
                if (childIndex < 0)
                {
                    break;
                }
                target = target._children[childIndex];
            }
            return target;
        }

        private void _TrySplit()
        {
            if (!CanSplit ||
                SelfCount < Settings.SplitThreshold)
            {
                return;
            }
            _Split();
        }
#endif

        //public void Add(Rect itemRect, T itemData)
        //{
        //    if (!CanSplit ||
        //        (!HasSplit && (SelfCount + 1) < Settings.SplitThreshold))
        //    {
        //        _AddToSelf(itemRect, itemData);
        //        return;
        //    }
        //    if (!HasSplit)
        //    {
        //        _Split();
        //    }
        //    _AddToChildOrSelf(itemRect, itemData);
        //}

        //private void _AddToChildOrSelf(Rect itemRect, T itemData)
        //{
        //    int childIndex = _TryGetChildIndex(itemRect);
        //    if (childIndex < 0)
        //    {
        //        _AddToSelf(itemRect, itemData);
        //    }
        //    else
        //    {
        //        _AddToChild(childIndex, itemRect, itemData);
        //    }
        //}

        private void _AddToSelf(Rect itemRect, T itemData)
        {
            _rects.Add(itemRect);
            _masks.Add(GetMaskForRect(itemRect));
            _data.Add(itemData);
        }

        //private void _AddToChild(int childIndex, Rect itemRect, T itemData)
        //{
        //    _children[childIndex].Add(itemRect, itemData);
        //}


        /// <summary>
        /// 
        /// <para>
        /// Refer to <see cref="SpecialChildIndex"/> for special return values and their meaning.
        /// </para>
        /// </summary>
        /// <param name="itemRect"></param>
        /// <returns></returns>
        private int _TryGetChildIndex(Rect itemRect)
        {
            if (!CanSplit)
            {
                return SpecialChildIndex.ChildlessNode;
            }
            _ComputeHorzVertBuckets(itemRect, out int minGX, out int maxGX, out int minGY, out int maxGY);
            if (minGX == maxGX && minGY == maxGY)
            {
                return minGY * Settings.HorzDiv + minGX;
            }
            if (itemRect.Width < itemRect.Height / Settings.HighAspectRatioCriteria ||
                itemRect.Height < itemRect.Width / Settings.HighAspectRatioCriteria)
            {
                return SpecialChildIndex.HighAspectRatio;
            }
            return SpecialChildIndex.Straddle;
        }

        private void _ComputeHorzVertBuckets(Rect itemRect, out int minGX, out int maxGX, out int minGY, out int maxGY)
        {
            // Note: the MulDiv operations 
            // (value * dh / w) and (value * dv / h)
            // reduces the safe integer range to be 
            // (int.MaxValue / dh) and (int.MaxValue / dv) respectively.
            int x0 = BoundingRect.X;
            int y0 = BoundingRect.Y;
            int dh = Settings.HorzDiv;
            int dv = Settings.VertDiv;
            int w = BoundingRect.Width;
            int h = BoundingRect.Height;
            int minX = itemRect.Left;
            int maxX = itemRect.Right - 1;
            int minY = itemRect.Top;
            int maxY = itemRect.Bottom - 1;
            minGX = (minX - x0) * dh / w;
            maxGX = (maxX - x0) * dh / w;
            minGY = (minY - y0) * dv / h;
            maxGY = (maxY - y0) * dv / h;
        }

        private Rect _ComputeChildRect(int childIndex)
        {
            // ====== WARNING ======
            // It hasn't been proven beyond doubt that this calculation
            // is consistent with _ComputeHorzVertBuckets(), 
            // such that an item rect will not be sent to a wrong child.
            // due to off-by-one rounding errors in the MulDiv operations.
            // ======
            int x0 = BoundingRect.X;
            int y0 = BoundingRect.Y;
            int w = BoundingRect.Width;
            int h = BoundingRect.Height;
            int dh = Settings.HorzDiv;
            int dv = Settings.VertDiv;
            int gx = childIndex % dh;
            int gy = childIndex / dh;
            int x1 = gx * w / dh + x0;
            int x2 = (gx + 1) * w / dh + x0;
            int y1 = gy * h / dv + y0;
            int y2 = (gy + 1) * h / dv + y0;
            return new Rect(x1, y1, x2 - x1, y2 - y1);
        }

        private void _Split()
        {
            // TODO 
            // allocate children
            // filter the self lists and send items to children
            // keep unsplittable (straddle) items on the list, then pack the list.
        }

        public RectNodeV3<T> Zoom(Rect zoomRect)
        {
            // Returns the node that encompasses the zoomRect.
            return this;
        }

        public int RecursiveCount()
        {
            int total = 0;
            foreach (RectNodeV3<T> node in GetRecursiveNodeList())
            {
                total += node._rects.Count;
            }
            return total;
        }

        public List<RectNodeV3<T>> GetRecursiveNodeList()
        {
            return GetRecursiveNodeList((RectNodeV3<T> _) => true);
        }

        public List<RectNodeV3<T>> GetRecursiveNodeList(Func<RectNodeV3<T>, bool> nodePredicate)
        {
            var result = new List<RectNodeV3<T>>();
            if (nodePredicate(this))
            {
                result.Add(this);
            }
            int nextIndex = 0;
            while (nextIndex < result.Count)
            {
                int indexToExpand = nextIndex++;
                RectNodeV3<T> childToExpand = result[indexToExpand];
                if (childToExpand._children is null)
                {
                    continue;
                }
                foreach (var grandChild in childToExpand._children)
                {
                    if (nodePredicate(grandChild))
                    {
                        result.Add(grandChild);
                    }
                }
            }
            return result;
        }

        public Rect? ValidateItemRect(Rect origItemRect)
        {
            return origItemRect;
        }

        public Rect? ValidateQueryRect(Rect origQueryRect)
        {
            return origQueryRect;
        }

        public RectMask128 GetMaskForRect(Rect rect)
        {
            return default;
        }
    }
}
#endif
