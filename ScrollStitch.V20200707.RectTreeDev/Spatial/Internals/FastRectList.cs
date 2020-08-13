using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Functional;
    using System.Collections;

    /// <summary>
    /// 
    /// </summary>
    public class FastRectList
        : IReadOnlyList<Rect>
        , IRectQuery<int>
    {
        private const int BitsPerAxis = 64; // for RectMask128
        private readonly int _stepSize;
        private readonly List<Rect> _rects;
        private readonly List<RectMask128> _masks;

        public Rect BoundingRect { get; }

        public int Count => _rects.Count;

        public int Capacity
        {
            get => _rects.Capacity;
            set
            {
                if (value >= _rects.Count)
                {
                    _rects.Capacity = value;
                    _masks.Capacity = value;
                }
            }
        }

        public Rect this[int index] => _rects[index];

        public FastRectList(Rect boundingRect)
            : this(boundingRect, capacity: 0)
        {
        }

        public FastRectList(Rect boundingRect, int capacity)
        {
            BoundingRect = boundingRect;
            int boundWidth = boundingRect.Width;
            int boundHeight = boundingRect.Height;
            if (boundWidth <= 0 ||
                boundHeight <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(boundingRect));
            }
            int maxBoundingLen = Math.Max(boundWidth, boundHeight);
            _stepSize = (maxBoundingLen + BitsPerAxis - 1) / BitsPerAxis;
            _rects = new List<Rect>(capacity: capacity);
            _masks = new List<RectMask128>(capacity: capacity);
        }

        /// <summary>
        /// Adds the rectangle to the list.
        /// </summary>
        /// 
        /// <returns>
        /// The integer index assigned to the rect. 
        /// <br/>
        /// This is an auto-incrementing value that starts at zero.
        /// </returns>
        /// 
        public int Add(Rect rect)
        {
            if (!RectMaskUtility.TryEncodeRect(BoundingRect, _stepSize, rect, out ulong xmask, out ulong ymask))
            {
                // all bits set, which forces a bruteforce comparison.
                xmask = ~0uL;
                ymask = ~0uL;
            }
            _rects.Add(rect);
            _masks.Add(new RectMask128(xmask, ymask));
            return _rects.Count - 1;
        }

        public void Clear()
        {
            _rects.Clear();
            _masks.Clear();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public IEnumerable<int> Enumerate(Rect queryRect)
        {
            if (!RectMaskUtility.TryEncodeRect(BoundingRect, _stepSize, queryRect, out ulong xmask, out ulong ymask))
            {
                // all bits set, which forces a bruteforce comparison.
                xmask = ~0uL;
                ymask = ~0uL;
            }
            RectMask128 queryMask = new RectMask128(xmask, ymask);
            int count = _masks.Count;
            for (int index = 0; index < count; ++index)
            {
                var itemMask = _masks[index];
                if (!default(RectMask128).Test(queryMask, itemMask))
                {
                    continue;
                }
                var itemRect = _rects[index];
                if (!InternalRectUtility.NoInline.HasIntersect(queryRect, itemRect))
                {
                    continue;
                }
                yield return index;
            }
        }

        public void ForEach<TFuncInline>(Rect queryRect, TFuncInline func)
            where TFuncInline : struct, IFuncInline<TFuncInline, int, Rect, bool>
        {
            if (!RectMaskUtility.TryEncodeRect(BoundingRect, _stepSize, queryRect, out ulong xmask, out ulong ymask))
            {
                // all bits set, which forces a bruteforce comparison.
                xmask = ~0uL;
                ymask = ~0uL;
            }
            RectMask128 queryMask = new RectMask128(xmask, ymask);
            int count = _masks.Count;
            for (int index = 0; index < count; ++index)
            {
                var itemMask = _masks[index];
                if (!default(RectMask128).Test(queryMask, itemMask))
                {
                    continue;
                }
                var itemRect = _rects[index];
                if (!InternalRectUtility.NoInline.HasIntersect(queryRect, itemRect))
                {
                    continue;
                }
                bool shouldContinue = func.Invoke(index, itemRect);
                if (!shouldContinue)
                {
                    return;
                }
            }
        }

        private struct FuncAdapter
            : IFuncInline<FuncAdapter, int, Rect, bool>
        {
            private readonly Func<int, Rect, bool> _func;

            internal FuncAdapter(Func<int, Rect, bool> func)
            {
                _func = func;
            }

            internal FuncAdapter(Func<int, bool> func)
            {
                _func = new Func<int, Rect, bool>(
                    (int index, Rect rect) => 
                    {
                        return func(index);
                    });
            }

            internal FuncAdapter(Func<Rect, bool> func)
            {
                _func = new Func<int, Rect, bool>(
                    (int index, Rect rect) =>
                    {
                        return func(rect);
                    });
            }

            internal FuncAdapter(Action<int, Rect> func)
            {
                _func = new Func<int, Rect, bool>(
                    (int index, Rect rect) =>
                    {
                        func(index, rect);
                        return true;
                    });
            }

            internal FuncAdapter(Action<int> func)
            {
                _func = new Func<int, Rect, bool>(
                    (int index, Rect rect) =>
                    {
                        func(index);
                        return true;
                    });
            }

            internal FuncAdapter(Action<Rect> func)
            {
                _func = new Func<int, Rect, bool>(
                    (int index, Rect rect) =>
                    {
                        func(rect);
                        return true;
                    });
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Invoke(int index, Rect rect)
            {
                return _func(index, rect);
            }
        }

        public void ForEach(Rect queryRect, Func<int, Rect, bool> func) 
        {
            ForEach(queryRect, new FuncAdapter(func));
        }

        public void ForEach(Rect queryRect, Func<int, bool> func)
        {
            ForEach(queryRect, new FuncAdapter(func));
        }

        public void ForEach(Rect queryRect, Func<Rect, bool> func)
        {
            ForEach(queryRect, new FuncAdapter(func));
        }

        public void ForEach(Rect queryRect, Action<int, Rect> func)
        {
            ForEach(queryRect, new FuncAdapter(func));
        }

        public void ForEach(Rect queryRect, Action<int> func)
        {
            ForEach(queryRect, new FuncAdapter(func));
        }

        public void ForEach(Rect queryRect, Action<Rect> func)
        {
            ForEach(queryRect, new FuncAdapter(func));
        }

        private class CountHelper
        {
            internal int Count;
        }

        private struct CountAdapter
            : IFuncInline<CountAdapter, int, Rect, bool>
        {
            private readonly CountHelper _countHelper;

            internal CountAdapter(CountHelper countHelper)
            {
                _countHelper = countHelper;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Invoke(int index, Rect rect)
            {
                _countHelper.Count += 1;
                return true;
            }
        }

        public int GetCount(Rect queryRect)
        {
            var helper = new CountHelper();
            ForEach(queryRect, new CountAdapter(helper));
            return helper.Count;
        }

        public IEnumerator<Rect> GetEnumerator()
        {
            return ((IEnumerable<Rect>)_rects).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_rects).GetEnumerator();
        }
    }
}
