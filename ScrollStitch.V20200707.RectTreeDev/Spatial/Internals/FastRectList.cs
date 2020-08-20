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

        public int Count => _GetCount_Validated();

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
            if (!boundingRect.IsPositive)
            {
                throw new ArgumentOutOfRangeException(nameof(boundingRect));
            }
            BoundingRect = boundingRect;
            _stepSize = _CtorComputeStepSize();
            _rects = new List<Rect>(capacity: capacity);
            _masks = new List<RectMask128>(capacity: capacity);
        }

        public FastRectList(Rect boundingRect, IEnumerable<Rect> rects)
            : this(boundingRect, capacity: _TryGetEnumerableCount(rects))
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
            if (!InternalRectUtility.Inline.ContainsWithin(BoundingRect, rect))
            {
                throw new ArgumentException(nameof(rect));
            }
            RectMaskUtility.TryEncodeRect(BoundingRect, _stepSize, rect, out ulong xmask, out ulong ymask);
            _rects.Add(rect);
            _masks.Add(new RectMask128(xmask, ymask));
            // ======
            // Returns the index of the most recently inserted item.
            // ------
            return _GetCount_Validated() - 1;
        }

        /// <summary>
        /// Adds all rectangles.
        /// </summary>
        /// <param name="rects">
        /// </param>
        /// 
        public void AddRange(IEnumerable<Rect> rects)
        {
            _ReserveNewSpace(_TryGetEnumerableCount(rects));
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
        /// Finds the index of the first rectangle for which the predicate returns true.
        /// </summary>
        /// <param name="rectPredicate"></param>
        /// <returns>
        /// The index of the first rectangle for which the predicate returns true, or 
        /// a negative value such as minus one <c>(-1)</c> if the predicate returns false for all of the rectangles.
        /// </returns>
        /// 
        public int FindFirst(Func<Rect, bool> rectPredicate)
        {
            int count = _GetCount_Validated();
            for (int index = 0; index < count; ++index)
            {
                if (rectPredicate(_rects[index]))
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first rectangle item for which the binary relation is true.
        /// 
        /// <para>
        /// This is an overload that takes an <see cref="IRectRelation"/>.
        /// </para>
        /// 
        /// <para>
        /// Specifically, it finds the first item that satisfies the following expression:
        /// <br/>
        /// <c>(relation.Test(itemRect, queryRect))</c>
        /// </para>
        /// </summary>
        /// 
        /// <returns>
        /// The index of the first item satisfying the binary relation with the query. 
        /// <br/>
        /// If no item satisfies the relation, a negative value <c>-1</c> is returned.
        /// </returns>
        /// 
        public int FindFirst(IRectRelation relation, Rect queryRect)
        {
            if (relation is null)
            {
                throw new ArgumentNullException(nameof(relation));
            }
            if (relation is IRectMaskRelation<RectMask128> maskRelation128)
            {
                return FindFirst(maskRelation128, queryRect);
            }
            int count = _GetCount_Validated();
            for (int index = 0; index < count; ++index)
            {
                if (relation.Test(_rects[index], queryRect))
                {
                    return index;
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first rectangle item for which the binary relation is true.
        /// 
        /// <para>
        /// This is an overload that takes an <see cref="IRectMaskRelation{TRectMask}/> of <see cref="RectMask128"/>.
        /// </para>
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
        public int FindFirst(IRectMaskRelation<RectMask128> relation, Rect queryRect)
        {
            if (relation is null)
            {
                throw new ArgumentNullException(nameof(relation));
            }
            switch (relation)
            {
                case RectRelations.Identical identical:
                    return FindFirst<RectRelations.Identical>(identical, queryRect);

                case RectRelations.IdenticalNT identicalNT:
                    return FindFirst<RectRelations.IdenticalNT>(identicalNT, queryRect);

                case RectRelations.Intersect intersect:
                    return FindFirst<RectRelations.Intersect>(intersect, queryRect);

                case RectRelations.Encompassing encompassing:
                    return FindFirst<RectRelations.Encompassing>(encompassing, queryRect);

                case RectRelations.EncompassingNT encompassingNT:
                    return FindFirst<RectRelations.EncompassingNT>(encompassingNT, queryRect);

                case RectRelations.EncompassedBy encompassedBy:
                    return FindFirst<RectRelations.EncompassedBy>(encompassedBy, queryRect);

                case RectRelations.EncompassedByNT encompassedByNT:
                    return FindFirst<RectRelations.EncompassedByNT>(encompassedByNT, queryRect);
                
                default:
                    break;
            }
            // ======
            // In some types of rectangular relation queries (specifically: triviality-allowing relations), 
            // a distinction needs to be made between the original query rect and the clamped query rect.
            // ------
            if (!(InternalRectUtility.Inline.TryComputeIntersection(BoundingRect, queryRect) is Rect clampedQueryRect))
            {
                return -1;
            }
            RectMask128 queryMask = _ConvertQueryRectToMask(clampedQueryRect);
            int count = _GetCount_Validated();
            for (int index = 0; index < count; ++index)
            {
                if (relation.TestMaybe(_masks[index], queryMask))
                {
                    if (relation.Test(_rects[index], queryRect))
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns the index of the first rectangle item for which the binary relation is true.
        /// 
        /// <para>
        /// This is an overload that takes an <see cref="IRectMaskRelationInline{TStruct, TRectMask}"/> 
        /// of <see cref="RectMask128"/>. 
        /// <br/>
        /// This overload forces the relation type to be known at compile-time and its bit mask testing 
        /// method to be available for inlining.
        /// </para>
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
        public int FindFirst<TRelation>(TRelation relation, Rect queryRect)
            where TRelation : struct, IRectMaskRelationInline<TRelation, RectMask128>
        {
            // ======
            // In some types of rectangular relation queries (specifically: triviality-allowing relations), 
            // a distinction needs to be made between the original query rect and the clamped query rect.
            // ------
            if (!(InternalRectUtility.Inline.TryComputeIntersection(BoundingRect, queryRect) is Rect clampedQueryRect))
            {
                return -1;
            }
            RectMask128 queryMask = _ConvertQueryRectToMask(clampedQueryRect);
            int count = _GetCount_Validated();
            for (int index = 0; index < count; ++index)
            {
                if (relation.TestMaybe(_masks[index], queryMask))
                {
                    if (relation.Test(_rects[index], queryRect))
                    {
                        return index;
                    }
                }
            }
            return -1;
        }

        public IEnumerable<int> Enumerate<TRelation>(TRelation relation, Rect queryRect)
            where TRelation : struct, IRectMaskRelationInline<TRelation, RectMask128>
        {
            RectMask128 queryMask = _ConvertQueryRectToMask(queryRect);
            return new HelperClasses.EnumeratorProvider<int>(() =>
            {
                return new HelperClasses.FilteredEnumerator<TRelation>(_rects, _masks, relation, queryRect, queryMask);
            });
        }

        /// <summary>
        /// Same as <see cref="FastRectListMethods.EnumerateIntersect(FastRectList, Rect)"/>.
        /// <br/>
        /// This function is retained for compatibility with <see cref="IRectQuery{T}"/>.
        /// </summary>
        /// 
        public IEnumerable<int> Enumerate(Rect queryRect)
        {
            return Enumerate(default(RectRelations.Intersect), queryRect);
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
        public void ForEach<TRelation, TFuncInline>(TRelation relation, Rect queryRect, TFuncInline func)
            where TRelation : struct, IRectMaskRelationInline<TRelation, RectMask128>
            where TFuncInline : struct, IFuncInline<TFuncInline, int, Rect, bool>
        {
            // ======
            // In some types of rectangular relation queries (specifically: triviality-allowing relations), 
            // a distinction needs to be made between the original query rect and the clamped query rect.
            // ------
            if (!(InternalRectUtility.Inline.TryComputeIntersection(BoundingRect, queryRect) is Rect clampedQueryRect))
            {
                return;
            }
            RectMask128 queryMask = _ConvertQueryRectToMask(clampedQueryRect);
            int count = _GetCount_Validated();
            for (int index = 0; index < count; ++index)
            {
                if (relation.TestMaybe(_masks[index], queryMask))
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
            var boxedCount = new HelperClasses.BoxedCount();
            ForEach(default(RectRelations.Intersect), queryRect, new HelperClasses.CountAdapter(boxedCount));
            return boxedCount.Count;
        }

        public IEnumerator<Rect> GetEnumerator()
        {
            _GetCount_Validated();
            return ((IEnumerable<Rect>)_rects).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            _GetCount_Validated();
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
            _GetCount_Validated();
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
            _GetCount_Validated();
            return _rects.ToArray();
        }

        private static int _TryGetEnumerableCount(IEnumerable<Rect> rects)
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

        private void _ReserveNewSpace(int additionalItemCount)
        {
            if (additionalItemCount <= 0)
            {
                return;
            }
            checked
            {
                int capacityAtLeast = _rects.Count + additionalItemCount;
                if (_rects.Capacity >= capacityAtLeast)
                {
                    return;
                }
                // Preserve the "grow by doubling" behavior.
                int targetCapacity = _rects.Capacity;
                while (targetCapacity < capacityAtLeast)
                {
                    targetCapacity *= 2;
                }
                _rects.Capacity = targetCapacity;
                _masks.Capacity = targetCapacity;
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
        private int _GetCount_Validated()
        {
            if (_rects is null ||
                _masks is null)
            {
                NoInline._ThrowClassInvariantViolation();
            }
            int count = _rects.Count;
            int maskCount = _masks.Count;
            if (count != maskCount)
            {
                NoInline._ThrowClassInvariantViolation();
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int _CtorComputeStepSize()
        {
            int maxBoundingLen = Math.Max(BoundingRect.Width, BoundingRect.Height);
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
                where TRelation : struct, IRectMaskRelationInline<TRelation, RectMask128>
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
                    if (rects is null || masks is null ||
                        rects.Count != masks.Count)
                    {
                        throw new ArgumentException();
                    }
                    _rects = rects;
                    _masks = masks;
                    _count = rects.Count;
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
