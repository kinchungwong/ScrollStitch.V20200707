using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.NewEnumeratorDesign2
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Spatial.Internals;
    using ScrollStitch.V20200707.Spatial.NewEnumeratorDesign;
    using ScrollStitch.V20200707.Spatial.NewEnumeratorDesign2;

    public class FilteredRectMaskEnumerator
        : IEnumerable<(Rect Rect, RectMask128 Mask)>
    {
        public IEnumerator<(Rect Rect, RectMask128 Mask)> GetEnumerator<TRelation>(TRelation relation, Rect queryRect)
            where TRelation : struct, IRectMaskRelationInline<TRelation, RectMask128>
        {
            RectMask128 queryMask = /*TOOD value*/ default;
            var pred = new RectMaskPredicateInline<TRelation>(relation, queryRect, queryMask);
#if true
            return FilteredEnumeratorBuilder.Create(GetEnumerator(), pred);
#else
            return new FilteredEnumeratorInline<
                RectMaskPredicateInline<TRelation>, (Rect Rect, RectMask128 Mask)>(
                GetEnumerator(), pred);
#endif
        }

        public IEnumerator<(Rect Rect, RectMask128 Mask)> GetEnumerator(IRectMaskRelation<RectMask128> relation, Rect queryRect)
        {
            RectMask128 queryMask = /*TOOD value*/ default;
            var pred = new RectMaskRelationPredicate(relation, queryRect, queryMask);
#if true
            return FilteredEnumeratorBuilder.Create(GetEnumerator(), pred);
#else
            return new FilteredEnumerable<(Rect Rect, RectMask128 Mask)>(this, pred.Invoke).GetEnumerator();
#endif
        }

        public IEnumerator<(Rect Rect, RectMask128 Mask)> GetEnumerator(Func<Rect, bool> rectPred)
        {
            bool RectMaskPred((Rect Rect, RectMask128 Mask) itemRectAndMask)
            {
                return rectPred(itemRectAndMask.Rect);
            }
#if true
            return FilteredEnumeratorBuilder.Create(GetEnumerator(), RectMaskPred);
#else
            return new FilteredEnumerable<(Rect Rect, RectMask128 Mask)>(this, RectMaskPred).GetEnumerator();
#endif
        }

        public IEnumerator<(Rect Rect, RectMask128 Mask)> GetEnumerator(IRectRelation relation, Rect queryRect)
        {
            if (relation is null)
            {
                throw new ArgumentNullException(nameof(relation));
            }
            // ======
            // Devirtualization table.
            // ======
            switch (relation)
            {
                case RectRelations.Identical identical:
                    return GetEnumerator(identical, queryRect);

                case RectRelations.IdenticalNT identicalNT:
                    return GetEnumerator(identicalNT, queryRect);

                case RectRelations.Intersect intersect:
                    return GetEnumerator(intersect, queryRect);

                case RectRelations.Encompassing encompassing:
                    return GetEnumerator(encompassing, queryRect);

                case RectRelations.EncompassingNT encompassingNT:
                    return GetEnumerator(encompassingNT, queryRect);

                case RectRelations.EncompassedBy encompassedBy:
                    return GetEnumerator(encompassedBy, queryRect);

                case RectRelations.EncompassedByNT encompassedByNT:
                    return GetEnumerator(encompassedByNT, queryRect);

                case IRectMaskRelation<RectMask128> rectMaskRelation:
                    // Accelerated cases, not found on known type list
                    return GetEnumerator(rectMaskRelation, queryRect);

                default:
                    // Non-accelerated case.
                    return GetEnumerator((Rect itemRect) => relation.Test(itemRect, queryRect));
            }
        }

        public IEnumerator<(Rect Rect, RectMask128 Mask)> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
