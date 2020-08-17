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
            _stepSize = _ComputeStepSize(boundWidth, boundHeight);
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
        /// Returns the index of the first rectangle item for which the binary relation is true.
        /// 
        /// <para>
        /// Specifically, it finds the first item that satisfies the following expression:
        /// <br/>
        /// <c>(relation.TestMaybe(itemMask, queryMask) &amp;&amp; relation.Test(itemRect, queryRect))</c>
        /// </para>
        /// </summary>
        /// 
        /// <returns>
        /// The index of the first item satisfying the binary relation with the query. 
        /// <br/>
        /// If no item satisfies the relation, a negative value <c>-1</c> is returned.
        /// </returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindFirst<TRelation>(TRelation relation, Rect queryRect, RectMask128 queryMask)
            where TRelation : struct, IRectRelation<TRelation, RectMask128>
        {
            // Make local copy of instance fields to avoid false aliasing in codegen
            var masks = _masks;
            var rects = _rects;
            int count = masks?.Count ?? 0;
            int rectCount = rects?.Count ?? 0;
            if (rectCount != count)
            {
                // Exception throwing is the caller's responsibility.
                return -1;
            }
            for (int index = 0; index < count; ++index)
            {
                var itemMask = masks[index];
                if (relation.TestMaybe(itemMask, queryMask))
                {
                    var itemRect = rects[index];
                    if (relation.Test(itemRect, queryRect))
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first rectangle item that is identical to the query rectangle.
        /// </summary>
        /// <param name="queryRect">
        /// The query rectangle.
        /// </param>
        /// <returns>
        /// The index of the first rectangle item that is identical to the query rectangle. <br/>
        /// If none of the rectangle items intersect, a negative value <c>-1</c> is returned.
        /// </returns>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindFirstIdentical(Rect queryRect)
        {
            var relation = default(RectRelations.IdenticalNT);
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            return FindFirst(relation, queryRect, queryMask);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindFirstIntersect(Rect queryRect)
        {
            var relation = default(RectRelations.Intersect);
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            return FindFirst(relation, queryRect, queryMask);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindFirstEncompassing(Rect queryRect)
        {
            var relation = default(RectRelations.EncompassingNT);
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            return FindFirst(relation, queryRect, queryMask);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int FindFirstEncompassedBy(Rect queryRect)
        {
            var relation = default(RectRelations.EncompassedByNT);
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            return FindFirst(relation, queryRect, queryMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<int> Enumerate<TRelation>(TRelation relation, Rect queryRect, RectMask128 queryMask)
            where TRelation : struct, IRectRelation<TRelation, RectMask128>
        {
            return new HelperClasses.EnumeratorProvider<int>(() =>
            {
                return new HelperClasses.FilteredEnumerator<TRelation>(_rects, _masks, relation, queryRect, queryMask);
            });
        }

        /// <summary>
        /// Enumerates all rectangle items that are identical to the non-empty query rectangle.
        /// 
        /// <para>
        /// See also: <br/>
        /// </para>
        /// <inheritdoc cref="RectRelations.IdenticalNT"/>
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<int> EnumerateIdentical(Rect queryRect)
        {
            var relation = default(RectRelations.IdenticalNT);
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            return Enumerate(relation, queryRect, queryMask);
        }

        /// <summary>
        /// Enumerates all rectangle items that intersect with the query rectangle.
        /// 
        /// <para>
        /// See also: <br/>
        /// </para>
        /// <inheritdoc cref="RectRelations.Intersect"/>
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<int> EnumerateIntersect(Rect queryRect)
        {
            var relation = default(RectRelations.Intersect);
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            return Enumerate(relation, queryRect, queryMask);
        }

        /// <summary>
        /// Same as <see cref="EnumerateIntersect(Rect)"/>.
        /// <br/>
        /// This function is retained for compatibility with <see cref="IRectQuery{T}"/>.
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<int> Enumerate(Rect queryRect)
        {
            return EnumerateIntersect(queryRect);
        }

        /// <summary>
        /// Enumerates all rectangle items that encompass the non-empty query rectangle.
        /// 
        /// <para>
        /// See also: <br/>
        /// </para>
        /// <inheritdoc cref="RectRelations.EncompassingNT"/>
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<int> EnumerateEncompassing(Rect queryRect)
        {
            var relation = default(RectRelations.EncompassingNT);
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            return Enumerate(relation, queryRect, queryMask);
        }

        /// <summary>
        /// Enumerates all rectangle items that are non-empty and encompassed by the query rectangle.
        /// 
        /// <para>
        /// See also: <br/>
        /// </para>
        /// <inheritdoc cref="RectRelations.EncompassedByNT"/>
        /// </summary>
        /// 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IEnumerable<int> EnumerateEncompassedBy(Rect queryRect)
        {
            var relation = default(RectRelations.EncompassedByNT);
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            return Enumerate(relation, queryRect, queryMask);
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ForEach<TRelation, TFuncInline>(TRelation relation, Rect queryRect, RectMask128 queryMask, TFuncInline func)
            where TRelation : struct, IRectRelation<TRelation, RectMask128>
            where TFuncInline : struct, IFuncInline<TFuncInline, int, Rect, bool>
        {
            if (_masks is null ||
                _rects is null ||
                _masks.Count != _rects.Count)
            {
                // Exception throwing is the caller's responsibility.
                return;
            }
            int count = _masks.Count;
            for (int index = 0; index < count; ++index)
            {
                var itemMask = _masks[index];
                if (relation.TestMaybe(itemMask, queryMask))
                {
                    var itemRect = _rects[index];
                    if (relation.Test(itemRect, queryRect))
                    {
                        bool shouldContinue = func.Invoke(index, itemRect);
                        if (!shouldContinue)
                        {
                            return;
                        }
                    }
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
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ForEach(Rect queryRect, Func<int, Rect, bool> func) 
        {
            var relation = default(RectRelations.Intersect);
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            ForEach(relation, queryRect, queryMask, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ForEach(Rect queryRect, Func<int, bool> func)
        {
            var relation = default(RectRelations.Intersect);
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            ForEach(relation, queryRect, queryMask, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ForEach(Rect queryRect, Func<Rect, bool> func)
        {
            var relation = default(RectRelations.Intersect);
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            ForEach(relation, queryRect, queryMask, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ForEach(Rect queryRect, Action<int, Rect> func)
        {
            var relation = default(RectRelations.Intersect);
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            ForEach(relation, queryRect, queryMask, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ForEach(Rect queryRect, Action<int> func)
        {
            var relation = default(RectRelations.Intersect);
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            ForEach(relation, queryRect, queryMask, new HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(Rect, Func{int, Rect, bool})"/>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void ForEach(Rect queryRect, Action<Rect> func)
        {
            var relation = default(RectRelations.Intersect);
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            ForEach(relation, queryRect, queryMask, new HelperClasses.FuncAdapter(func));
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
        [MethodImpl(MethodImplOptions.NoInlining)]
        public int GetCount(Rect queryRect)
        {
            var relation = default(RectRelations.Intersect);
            _CheckClassInvariantElseThrow();
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            var boxedCount = new HelperClasses.BoxedCount();
            ForEach(relation, queryRect, queryMask, new HelperClasses.CountAdapter(boxedCount));
            return boxedCount.Count;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _CheckClassInvariantElseThrow()
        {
            if (_rects is null ||
                _masks is null ||
                _rects.Count != _masks.Count)
            {
                NoInline._ThrowClassInvariantViolation();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int _ComputeStepSize(int boundWidth, int boundHeight)
        {
            int maxBoundingLen = Math.Max(boundWidth, boundHeight);
            int stepSize = (maxBoundingLen + BitsPerAxis - 1) / BitsPerAxis;
            return stepSize;
        }

        private static class NoInline
        {
            [MethodImpl(MethodImplOptions.NoInlining)]
            public static void _ThrowClassInvariantViolation()
            {
                throw new Exception("Class invariant violation.");
            }
        }

        public static class HelperClasses
        {
            #region custom enumerators
            /// <summary>
            /// A class that provides an <see cref="IEnumerator{T0}"/> upon request. This class is thus used to 
            /// satisfy <see cref="IEnumerable{T0}"/>, either for a method or for the class itself.
            /// </summary>
            /// <typeparam name="T0"></typeparam>
            public struct EnumeratorProvider<T0>
                : IEnumerable<T0>
            {
                private Func<IEnumerator<T0>> _enumeratorCreateFunc;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public EnumeratorProvider(Func<IEnumerator<T0>> enumeratorCreateFunc)
                {
                    _enumeratorCreateFunc = enumeratorCreateFunc;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public IEnumerator<T0> GetEnumerator()
                {
                    return _enumeratorCreateFunc();
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }

            /// <summary>
            /// Non-relation-filtering enumerator, returning VT(int, Rect, RectMask128).
            /// </summary>
            public class Enumerator
                : IEnumerator<(int index, Rect rect, RectMask128 mask)> /*default enumerator*/
                , IEnumerator<int>
                , IEnumerator<Rect>
                , IEnumerator<KeyValuePair<int, Rect>>
            {
                private List<Rect> _rects;
                private List<RectMask128> _masks;
                private int _count;
                private int _index;

                [MethodImpl(MethodImplOptions.NoInlining)]
                public Enumerator(List<Rect> rects, List<RectMask128> masks)
                {
                    _rects = rects;
                    _masks = masks;
                    _count = rects?.Count ?? 0;
                    _index = -1;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool MoveNext()
                {
                    if (_index < 0)
                    {
                        _index = 0;
                    }
                    else if (_index < _count)
                    {
                        ++_index;
                    }
                    return (_index < _count);
                }

                public (int index, Rect rect, RectMask128 mask) Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get
                    {
                        bool shouldThrow = unchecked((uint)_index < (uint)_count);
                        if (shouldThrow)
                        {
                            return _Throw_FakeReturn();
                        }
                        return (index: _index, rect: _rects[_index], mask: _masks[_index]);
                    }
                }

                object IEnumerator.Current
                {
                    get => Current;
                }

                int IEnumerator<int>.Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => Current.index;
                }

                Rect IEnumerator<Rect>.Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => Current.rect;
                }
                KeyValuePair<int, Rect> IEnumerator<KeyValuePair<int, Rect>>.Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get
                    {
                        var current = Current;
                        return new KeyValuePair<int, Rect>(current.index, current.rect);
                    }
                }

                public void Dispose()
                {
                    _rects = null;
                    _masks = null;
                    _count = 0;
                    _index = -1;
                }

                public void Reset()
                {
                    _index = -1;
                }

                [MethodImpl(MethodImplOptions.NoInlining)]
                private static ValueTuple<int, Rect, RectMask128> _Throw_FakeReturn()
                {
                    throw new InvalidOperationException();
                }
            }

            /// <summary>
            /// Relation-filtering enumerator, returning VT(int, Rect, RectMask128).
            /// </summary>
            public class FilteredEnumerator<TRelation>
                : IEnumerator<(int index, Rect rect, RectMask128 mask)> /*default enumerator*/
                , IEnumerator<int>
                , IEnumerator<Rect>
                , IEnumerator<KeyValuePair<int, Rect>>
                where TRelation : struct, IRectRelation<TRelation, RectMask128>
            {
                private List<Rect> _rects;
                private List<RectMask128> _masks;
                private int _count;
                private int _index;
                private TRelation _relation;
                private Rect _secondRect;
                private RectMask128 _secondMask;

                [MethodImpl(MethodImplOptions.NoInlining)]
                public FilteredEnumerator(List<Rect> rects, List<RectMask128> masks, TRelation relation, Rect secondRect, RectMask128 secondMask)
                {
                    _rects = rects;
                    _masks = masks;
                    _count = rects?.Count ?? 0;
                    _index = -1;
                    _relation = relation;
                    _secondRect = secondRect;
                    _secondMask = secondMask;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool MoveNext()
                {
                    if (_count == 0)
                    {
                        _index = 0;
                        return false;
                    }
                    if (_index < 0)
                    {
                        _index = -1;
                    }
                    while (unchecked((uint)(_index + 1) < (uint)_count))
                    {
                        _index += 1;
                        RectMask128 itemMask = _masks[_index];
                        if (_relation.TestMaybe(itemMask, _secondMask))
                        {
                            Rect itemRect = _rects[_index];
                            if (_relation.Test(itemRect, _secondRect))
                            {
                                return true;
                            }
                        }
                    }
                    return false;
                }

                public (int index, Rect rect, RectMask128 mask) Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get
                    {
                        if (unchecked((uint)_index < (uint)_count))
                        {
                            return (index: _index, rect: _rects[_index], mask: _masks[_index]);
                        }
                        return _Throw_FakeReturn();
                    }
                }

                object IEnumerator.Current
                {
                    get => Current;
                }

                int IEnumerator<int>.Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => Current.index;
                }

                Rect IEnumerator<Rect>.Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get => Current.rect;
                }

                KeyValuePair<int, Rect> IEnumerator<KeyValuePair<int, Rect>>.Current
                {
                    [MethodImpl(MethodImplOptions.AggressiveInlining)]
                    get
                    {
                        var current = Current;
                        return new KeyValuePair<int, Rect>(current.index, current.rect);
                    }
                }

                public void Dispose()
                {
                    _rects = null;
                    _masks = null;
                    _count = 0;
                    _index = -1;
                    _relation = default;
                }

                public void Reset()
                {
                    _index = -1;
                }

                [MethodImpl(MethodImplOptions.NoInlining)]
                private static ValueTuple<int, Rect, RectMask128> _Throw_FakeReturn()
                {
                    throw new InvalidOperationException();
                }
            }
            #endregion

            /// <summary>
            /// <see cref="BoxedCount"/> is a reference type (class) that houses a mutable 
            /// integer value, <see cref="Count"/>.
            /// </summary>
            internal class BoxedCount
            {
                internal int Count;
            }

            internal struct CountAdapter
                : IFuncInline<CountAdapter, int, Rect, bool>
            {
                private readonly BoxedCount _boxedCount;

                internal CountAdapter(BoxedCount countHelper)
                {
                    _boxedCount = countHelper;
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public bool Invoke(int index, Rect rect)
                {
                    _boxedCount.Count += 1;
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
