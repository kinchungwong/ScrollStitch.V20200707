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

    /// <summary>
    /// <see cref="FastRectList"/> is a list of rectangles with support for <see cref="IRectQuery{T}"/>.
    /// 
    /// <para>
    /// Internally, this class assigns a bit mask for each rectangle on the list. The bit mask speeds up
    /// rectangle overlap testing.
    /// </para>
    /// 
    /// <para>
    /// This class requires a <see cref="BoundingRect"/> parameter. The insertion of rectangle items that 
    /// do not fit within this bounding rectangle will severely degrade query performance.
    /// </para>
    /// 
    /// <para>
    /// In order to maintain consistency between the bit masks and rectangles, this class does not expose 
    /// mutable access to the internal list.
    /// </para>
    /// </summary>
    /// 
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

        public FastRectList(Rect boundingRect, IEnumerable<Rect> rects)
            : this(boundingRect, capacity: _CtorTryGetCount(rects))
        {
            AddRange(rects);
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
            _CheckClassInvariantElseThrow();
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

        /// <summary>
        /// Adds all rectangles.
        /// </summary>
        /// <param name="rects">
        /// </param>
        /// 
        public void AddRange(IEnumerable<Rect> rects)
        {
            foreach (var rect in rects)
            {
                Add(rect);
            }
        }

        public void Clear()
        {
            _rects.Clear();
            _masks.Clear();
        }

        /// <summary>
        /// Returns the index of the first rectangle item that intersects with the query rectangle.
        /// </summary>
        /// <param name="queryRect">
        /// The query rectangle.
        /// </param>
        /// <returns>
        /// The index of the first rectangle item that intersects with the query rectangle. <br/>
        /// If none of the rectangle items intersect, a negative value <c>-1</c> is returned.
        /// </returns>
        /// 
        public int FindFirstIntersect(Rect queryRect)
        {
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            int count = _masks.Count;
            for (int index = 0; index < count; ++index)
            {
                var itemMask = _masks[index];
                if (!queryMask.MaybeIntersecting(itemMask))
                {
                    continue;
                }
                var itemRect = _rects[index];
                if (!InternalRectUtility.NoInline.HasIntersect(queryRect, itemRect))
                {
                    continue;
                }
                return index;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first rectangle item that encompasses the query rectangle.
        /// </summary>
        /// <param name="queryRect">
        /// The query rectangle.
        /// </param>
        /// <returns>
        /// The index of the first rectangle item that encompasses the query rectangle. <br/>
        /// If none of the rectangle items encompasses the query rectangle, a negative value 
        /// <c>-1</c> is returned.
        /// </returns>
        /// 
        public int FindFirstEncompassing(Rect queryRect)
        {
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            int count = _masks.Count;
            for (int index = 0; index < count; ++index)
            {
                var itemMask = _masks[index];
                if (!itemMask.MaybeEncompassingNT(queryMask))
                {
                    continue;
                }
                var itemRect = _rects[index];
                if (!InternalRectUtility.NoInline.ContainsWithin(rectOuter: itemRect, rectInner: queryRect))
                {
                    continue;
                }
                return index;
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first rectangle item that is encompassed by the query rectangle.
        /// </summary>
        /// <param name="queryRect">
        /// The query rectangle.
        /// </param>
        /// <returns>
        /// The index of the first rectangle item that is encompassed by the query rectangle. <br/>
        /// If none of the rectangle items are encompassed by the query rectangle, a negative value 
        /// <c>-1</c> is returned.
        /// </returns>
        /// 
        public int FindFirstEncompassedBy(Rect queryRect)
        {
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            int count = _masks.Count;
            for (int index = 0; index < count; ++index)
            {
                var itemMask = _masks[index];
                if (!queryMask.MaybeEncompassingNT(itemMask))
                {
                    continue;
                }
                var itemRect = _rects[index];
                if (!InternalRectUtility.NoInline.ContainsWithin(rectOuter: queryRect, rectInner: itemRect))
                {
                    continue;
                }
                return index;
            }
            return -1;
        }

        /// <summary>
        /// Enumerates all rectangle items that intersect with the query rectangle.
        /// </summary>
        /// 
        /// <param name="queryRect">
        /// The query rectangle.
        /// </param>
        /// 
        /// <returns>
        /// An enumeration of all rectangle items that intersect with the query rectangle.
        /// </returns>
        /// 
        public IEnumerable<int> Enumerate(Rect queryRect)
        {
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            int count = _masks.Count;
            for (int index = 0; index < count; ++index)
            {
                var itemMask = _masks[index];
                if (!queryMask.MaybeIntersecting(itemMask))
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

        /// <summary>
        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        /// 
        /// <para>
        /// The following sections only apply to the special overload which accepts <see cref="IFuncInline"/>.
        /// 
        /// <para>
        /// (Applies to <see cref="IFuncInline"/> only) The functor type is constrained to be a struct.
        /// </para>
        /// 
        /// <para>
        /// (Applies to <see cref="IFuncInline"/> only) The functor's <c>Invoke</c> method shall be marked 
        /// <see cref="MethodImplOptions.AggressiveInlining"/>.
        /// </para>
        /// 
        /// <para>
        /// (Applies to <see cref="IFuncInline"/> only) 
        /// If the struct needs to capture mutable state, it is imperative that such state be stored separately 
        /// (on an instance of a class). This is due to the fact that the functor type is restricted to be a 
        /// value type, which means changes made to the functor instance used inside this 
        /// <see cref="ForEach{TFuncInline}"/> method are not being propagated back to the caller's functor 
        /// instance.
        /// </para>
        /// </para>
        /// </summary>
        /// 
        /// <typeparam name="TFuncInline">
        /// The concrete type that implements <see cref="IFuncInline"/>. 
        /// </typeparam>
        /// 
        /// <param name="queryRect">
        /// The query rectangle.
        /// </param>
        /// 
        /// <param name="func">
        /// 
        /// </param>
        /// 
        public void ForEach<TFuncInline>(Rect queryRect, TFuncInline func)
            where TFuncInline : struct, IFuncInline<TFuncInline, int, Rect, bool>
        {
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            int count = _masks.Count;
            for (int index = 0; index < count; ++index)
            {
                var itemMask = _masks[index];
                if (!queryMask.MaybeIntersecting(itemMask))
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

        /// <summary>
        /// Calls the specified function for each rectangle item that intersects with the query rectangle.
        /// 
        /// <para>
        /// There are several <c>ForEach</c> overloads: <br/>
        /// <see cref="ForEach(Rect, Action{Rect})"/> <br/>
        /// <see cref="ForEach(Rect, Action{int, Rect})"/> <br/>
        /// <see cref="ForEach(Rect, Action{int})"/> <br/>
        /// <see cref="ForEach(Rect, Func{Rect, bool})"/> <br/>
        /// <see cref="ForEach(Rect, Func{int, Rect, bool})"/> <br/>
        /// <see cref="ForEach(Rect, Func{int, bool})"/>
        /// </para>
        /// 
        /// <para>
        /// The callback's integer parameter refers to the item index on the list. The item index helps 
        /// disambiguate between duplicate rectangles on the list.
        /// </para>
        /// 
        /// <para>
        /// Overloads which accept a <c>Func&lt;..., bool&gt;</c> can return a bool to indicate continuation. 
        /// The query loop can be stopped early by returning false.
        /// </para>
        /// </summary>
        /// 
        public void ForEach(Rect queryRect, Func<int, Rect, bool> func) 
        {
            ForEach(queryRect, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        public void ForEach(Rect queryRect, Func<int, bool> func)
        {
            ForEach(queryRect, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        public void ForEach(Rect queryRect, Func<Rect, bool> func)
        {
            ForEach(queryRect, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        public void ForEach(Rect queryRect, Action<int, Rect> func)
        {
            ForEach(queryRect, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        public void ForEach(Rect queryRect, Action<int> func)
        {
            ForEach(queryRect, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        public void ForEach(Rect queryRect, Action<Rect> func)
        {
            ForEach(queryRect, new HelperClasses.FuncAdapter(func));
        }

        /// <summary>
        /// Counts the number of rectangles that overlaps with the specified query rectangle.
        /// </summary>
        /// <param name="queryRect">
        /// The query rectangle.
        /// </param>
        /// <returns>
        /// The number of rectangle items on the list which overlap with the query rectangle.
        /// </returns>
        public int GetCount(Rect queryRect)
        {
            var helper = new HelperClasses.CountHelper();
            ForEach(queryRect, new HelperClasses.CountAdapter(helper));
            return helper.Count;
        }

        public IEnumerator<Rect> GetEnumerator()
        {
            _CheckClassInvariantElseThrow();
            return ((IEnumerable<Rect>)_rects).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            _CheckClassInvariantElseThrow();
            return ((IEnumerable)_rects).GetEnumerator();
        }

        /// <summary>
        /// Requests read-only access to the list of rectangles.
        /// 
        /// <para>
        /// This is an <c>O(1)</c> operation.
        /// </para>
        /// 
        /// <para>
        /// Changes made through the <see cref="FastRectList"/> instance could be visible through
        /// instances of <see cref="IReadOnlyList{T}"/> returned from this method.
        /// </para>
        /// </summary>
        /// 
        /// <returns>
        /// A read-only view into the list of rectangles.
        /// </returns>
        /// 
        public IReadOnlyList<Rect> AsReadOnly()
        {
            _CheckClassInvariantElseThrow();
            return _rects.AsReadOnly();
        }

        /// <summary>
        /// Copies all rectangles to an array.
        /// </summary>
        /// 
        /// <returns>
        /// An array containing a copy of every rectangle on the list.
        /// </returns>
        /// 
        public Rect[] ToArray()
        {
            _CheckClassInvariantElseThrow();
            return _rects.ToArray();
        }

        private static int _CtorTryGetCount(IEnumerable<Rect> rects)
        {
            switch (rects)
            {
                case null:
                    return 0;
                case ICollection<Rect> coll:
                    return coll.Count;
                case IReadOnlyCollection<Rect> rocoll:
                    return rocoll.Count;
                default:
                    return 0;
            }
        }

        private RectMask128 _ConvertQueryRectToMask(Rect queryRect)
        {
            if (!RectMaskUtility.TryEncodeRect(BoundingRect, _stepSize, queryRect, out ulong xmask, out ulong ymask))
            {
                // all bits set, which forces a bruteforce comparison.
                xmask = ~0uL;
                ymask = ~0uL;
            }
            RectMask128 queryMask = new RectMask128(xmask, ymask);
            return queryMask;
        }

        private void _CheckClassInvariantElseThrow()
        {
            if (_rects is null ||
                _masks is null ||
                _rects.Count != _masks.Count)
            {
                throw new Exception("Class invariant violation.");
            }
        }

        private static class HelperClasses
        {
            internal class CountHelper
            {
                internal int Count;
            }

            internal struct CountAdapter
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

            internal struct FuncAdapter
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
        }
    }
}
