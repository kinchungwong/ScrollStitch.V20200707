using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.NewEnumeratorDesign
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Functional;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Spatial.Internals;

    public class RectMaskRelationPredicate
        : IFunc<Rect, RectMask128, bool>
        , IFunc<(Rect Rect, RectMask128 Mask), bool>
    {
        public IRectMaskRelation<RectMask128> Relation { get; }

        public Rect SecondRect { get; }

        public RectMask128 SecondMask { get; }

        public RectMaskRelationPredicate(IRectMaskRelation<RectMask128> relation, Rect secondRect, RectMask128 secondMask)
        {
            Relation = relation;
            SecondRect = secondRect;
            SecondMask = secondMask;
        }

        public bool Invoke(Rect firstRect, RectMask128 firstMask)
        {
            if (!Relation.TestMaybe(firstMask, SecondMask))
            {
                return false;
            }
            if (!Relation.Test(firstRect, SecondRect))
            {
                return false;
            }
            return true;
        }

        public bool Invoke((Rect Rect, RectMask128 Mask) firstRectAndMask)
        {
            return Invoke(firstRectAndMask.Rect, firstRectAndMask.Mask);
        }
    }
}
