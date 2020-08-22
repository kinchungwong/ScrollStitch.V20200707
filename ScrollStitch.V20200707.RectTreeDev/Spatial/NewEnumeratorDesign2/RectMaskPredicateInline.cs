using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.NewEnumeratorDesign2
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Functional;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Spatial.Internals;

    public struct RectMaskPredicateData
    {
        public IRectMaskRelation<RectMask128> Relation { get; }

        public Rect SecondRect { get; }

        public RectMask128 SecondMask { get; }

        public RectMaskPredicateData(IRectMaskRelation<RectMask128> relation, Rect secondRect, RectMask128 secondMask)
        {
            Relation = relation;
            SecondRect = secondRect;
            SecondMask = secondMask;
        }
    }

    public struct RectMaskPredicateInline<TRelation>
        : IFuncInline<RectMaskPredicateInline<TRelation>, (Rect Rect, RectMask128 Mask), bool>
        where TRelation : struct, IRectMaskRelationInline<TRelation, RectMask128>
    {
        public TRelation Relation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public Rect SecondRect
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public RectMask128 SecondMask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public RectMaskPredicateInline(TRelation relation, Rect secondRect, RectMask128 secondMask)
        {
            Relation = relation;
            SecondRect = secondRect;
            SecondMask = secondMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Invoke((Rect Rect, RectMask128 Mask) firstRectAndMask)
        {
            if (!Relation.TestMaybe(firstRectAndMask.Mask, SecondMask))
            {
                return false;
            }
            if (!Relation.Test(firstRectAndMask.Rect, SecondRect))
            {
                return false;
            }
            return true;
        }
    }
}
