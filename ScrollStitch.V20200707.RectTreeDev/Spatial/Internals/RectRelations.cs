﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// <see cref="RectRelations"/> is a static class containing types that implement 
    /// <see cref="IRectRelation{TStruct, TRectMask}"/>, which describes binary relations
    /// for two rectangles.
    /// 
    /// <para>
    /// Some of these binary relations are non-symmetric. Examples of non-symmetric relations are: 
    /// <br/>
    /// <see cref="Encompassing"/>, <see cref="EncompassedBy"/>, and their triviality-rejecting 
    /// counterparts.
    /// <br/>
    /// For these non-symmetric relations, swapping the order of the two argument will change the 
    /// semantics of the relation.
    /// </para>
    /// 
    /// <para>
    /// Relations with the suffix <c>NT</c> refers to non-triviality. These relations reject the 
    /// empty rectangle (non-positive rectangle) as a trivial case, and will return false on both
    /// the bit mask tests and the actual rectangle tests.
    /// </para>
    /// 
    /// <para>
    /// Currently defined binary relations: <br/>
    /// </para>
    /// 
    /// <inheritdoc cref="Identical"/> <br/>
    /// <inheritdoc cref="IdenticalNT"/> <br/>
    /// <inheritdoc cref="Intersect"/> <br/>
    /// <inheritdoc cref="Encompassing"/> <br/>
    /// <inheritdoc cref="EncompassingNT"/> <br/>
    /// <inheritdoc cref="EncompassedBy"/> <br/>
    /// <inheritdoc cref="EncompassedByNT"/> <br/>
    /// </summary>
    public static class RectRelations
    {
        /// <summary>
        /// <see cref="Identical"/> tests whether the two rectangles are identical. 
        /// The trivial case (both rectangles are empty) is allowed.
        /// </summary>
        public struct Identical
            : IRectMaskRelation<Identical, RectMask128>
            , IRectMaskRelation<Identical, RectMask64>
            , IRectMaskRelation<Identical, RectMask32>
            , IRectMaskRelation<Identical, RectMask16>
            , IRectMaskRelation<Identical, RectMask8>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Test(Rect rectFirst, Rect rectSecond)
            {
                return rectFirst == rectSecond;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask128 maskFirst, RectMask128 maskSecond)
            {
                return maskFirst.XValue == maskSecond.XValue &&
                    maskFirst.YValue == maskSecond.YValue;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask64 maskFirst, RectMask64 maskSecond)
            {
                return maskFirst.XValue == maskSecond.XValue &&
                    maskFirst.YValue == maskSecond.YValue;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask32 maskFirst, RectMask32 maskSecond)
            {
                return maskFirst.XValue == maskSecond.XValue &&
                    maskFirst.YValue == maskSecond.YValue;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask16 maskFirst, RectMask16 maskSecond)
            {
                return maskFirst.XValue == maskSecond.XValue &&
                    maskFirst.YValue == maskSecond.YValue;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask8 maskFirst, RectMask8 maskSecond)
            {
                return maskFirst.XYValue == maskSecond.XYValue;
            }
        }

        /// <summary>
        /// <see cref="IdenticalNT"/> tests whether the two rectangles are identical. 
        /// The trivial case (both rectangles are empty) is disallowed.
        /// </summary>
        public struct IdenticalNT
            : IRectMaskRelation<IdenticalNT, RectMask128>
            , IRectMaskRelation<IdenticalNT, RectMask64>
            , IRectMaskRelation<IdenticalNT, RectMask32>
            , IRectMaskRelation<IdenticalNT, RectMask16>
            , IRectMaskRelation<IdenticalNT, RectMask8>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Test(Rect rectFirst, Rect rectSecond)
            {
                return rectFirst == rectSecond &&
                    rectFirst.Width > 0 &&
                    rectFirst.Height > 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask128 maskFirst, RectMask128 maskSecond)
            {
                return maskFirst.XValue == maskSecond.XValue &&
                    maskFirst.YValue == maskSecond.YValue &&
                    maskFirst.XValue != 0ul &&
                    maskFirst.YValue != 0uL;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask64 maskFirst, RectMask64 maskSecond)
            {
                return maskFirst.XValue == maskSecond.XValue &&
                    maskFirst.YValue == maskSecond.YValue &&
                    maskFirst.XValue != 0u &&
                    maskFirst.YValue != 0u;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask32 maskFirst, RectMask32 maskSecond)
            {
                return maskFirst.XValue == maskSecond.XValue &&
                    maskFirst.YValue == maskSecond.YValue &&
                    maskFirst.XValue != 0u &&
                    maskFirst.YValue != 0u;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask16 maskFirst, RectMask16 maskSecond)
            {
                return maskFirst.XValue == maskSecond.XValue &&
                    maskFirst.YValue == maskSecond.YValue &&
                    maskFirst.XValue != 0u &&
                    maskFirst.YValue != 0u;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask8 maskFirst, RectMask8 maskSecond)
            {
                return maskFirst.XYValue == maskSecond.XYValue &&
                    (maskFirst.XYValue & 0x0F) != 0 &&
                    (maskFirst.XYValue & 0xF0) != 0;
            }
        }

        /// <summary>
        /// <see cref="Intersect"/> tests whether the two rectangles have a positive intersection. 
        /// This relation does not have a trivial case.
        /// </summary>
        public struct Intersect
            : IRectMaskRelation<Intersect, RectMask128>
            , IRectMaskRelation<Intersect, RectMask64>
            , IRectMaskRelation<Intersect, RectMask32>
            , IRectMaskRelation<Intersect, RectMask16>
            , IRectMaskRelation<Intersect, RectMask8>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Test(Rect rectFirst, Rect rectSecond)
            {
                return InternalRectUtility.Inline.TryComputeIntersection(rectFirst, rectSecond).HasValue;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask128 maskFirst, RectMask128 maskSecond)
            {
                return maskFirst.MaybeIntersecting(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask64 maskFirst, RectMask64 maskSecond)
            {
                return maskFirst.MaybeIntersecting(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask32 maskFirst, RectMask32 maskSecond)
            {
                return maskFirst.MaybeIntersecting(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask16 maskFirst, RectMask16 maskSecond)
            {
                return maskFirst.MaybeIntersecting(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask8 maskFirst, RectMask8 maskSecond)
            {
                return maskFirst.MaybeIntersecting(maskSecond);
            }
        }

        /// <summary>
        /// <see cref="Encompassing"/> tests whether the first rectangle encompasses the second (the inner). 
        /// The trivial case is allowed. 
        /// </summary>
        public struct Encompassing
            : IRectMaskRelation<Encompassing, RectMask128>
            , IRectMaskRelation<Encompassing, RectMask64>
            , IRectMaskRelation<Encompassing, RectMask32>
            , IRectMaskRelation<Encompassing, RectMask16>
            , IRectMaskRelation<Encompassing, RectMask8>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Test(Rect rectFirst, Rect rectSecond)
            {
                if (rectFirst.Width <= 0 || rectFirst.Height <= 0 ||
                    rectSecond.Width <= 0 || rectSecond.Height <= 0)
                {
                    // ======
                    // The triviality case.
                    // See EncompassingNT for the implementation that rejects the triviality case.
                    // (This explicit check is needed because ContainsWithin will throw when 
                    // either rect is non-positive.)
                    // ------
                    return true; 
                }
                return InternalRectUtility.Inline.ContainsWithin(rectOuter: rectFirst, rectInner: rectSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask128 maskFirst, RectMask128 maskSecond)
            {
                return maskFirst.MaybeEncompassing(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask64 maskFirst, RectMask64 maskSecond)
            {
                return maskFirst.MaybeEncompassing(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask32 maskFirst, RectMask32 maskSecond)
            {
                return maskFirst.MaybeEncompassing(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask16 maskFirst, RectMask16 maskSecond)
            {
                return maskFirst.MaybeEncompassing(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask8 maskFirst, RectMask8 maskSecond)
            {
                return maskFirst.MaybeEncompassing(maskSecond);
            }
        }

        /// <summary>
        /// <see cref="EncompassingNT"/> tests whether the first rectangle encompasses the second (the inner). 
        /// The trivial case is disallowed. 
        /// </summary>
        public struct EncompassingNT
            : IRectMaskRelation<EncompassingNT, RectMask128>
            , IRectMaskRelation<EncompassingNT, RectMask64>
            , IRectMaskRelation<EncompassingNT, RectMask32>
            , IRectMaskRelation<EncompassingNT, RectMask16>
            , IRectMaskRelation<EncompassingNT, RectMask8>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Test(Rect rectFirst, Rect rectSecond)
            {
                if (rectFirst.Width <= 0 || rectFirst.Height <= 0 ||
                    rectSecond.Width <= 0 || rectSecond.Height <= 0)
                {
                    // ======
                    // The triviality case, rejected.
                    // See Encompassing for the implementation that accepts the triviality case.
                    // (This explicit check is needed because ContainsWithin will throw when 
                    // either rect is non-positive.)
                    // ------
                    return false;
                }
                return InternalRectUtility.Inline.ContainsWithin(rectOuter: rectFirst, rectInner: rectSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask128 maskFirst, RectMask128 maskSecond)
            {
                return maskFirst.MaybeEncompassingNT(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask64 maskFirst, RectMask64 maskSecond)
            {
                return maskFirst.MaybeEncompassingNT(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask32 maskFirst, RectMask32 maskSecond)
            {
                return maskFirst.MaybeEncompassingNT(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask16 maskFirst, RectMask16 maskSecond)
            {
                return maskFirst.MaybeEncompassingNT(maskSecond);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask8 maskFirst, RectMask8 maskSecond)
            {
                return maskFirst.MaybeEncompassingNT(maskSecond);
            }
        }

        /// <summary>
        /// <see cref="EncompassedBy"/> tests whether the first rectangle is encompassed by the second (the outer). 
        /// The trivial case is allowed. 
        /// </summary>
        public struct EncompassedBy
            : IRectMaskRelation<EncompassedBy, RectMask128>
            , IRectMaskRelation<EncompassedBy, RectMask64>
            , IRectMaskRelation<EncompassedBy, RectMask32>
            , IRectMaskRelation<EncompassedBy, RectMask16>
            , IRectMaskRelation<EncompassedBy, RectMask8>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Test(Rect rectFirst, Rect rectSecond)
            {
                if (rectFirst.Width <= 0 || rectFirst.Height <= 0 ||
                    rectSecond.Width <= 0 || rectSecond.Height <= 0)
                {
                    // ======
                    // The triviality case.
                    // See EncompassedByNT for the implementation that rejects the triviality case.
                    // (This explicit check is needed because ContainsWithin will throw when 
                    // either rect is non-positive.)
                    // ------
                    return true;
                }
                return InternalRectUtility.Inline.ContainsWithin(rectOuter: rectSecond, rectInner: rectFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask128 maskFirst, RectMask128 maskSecond)
            {
                // ======
                // See class comment for proper argument ordering.
                // ------
                return maskSecond.MaybeEncompassing(maskFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask64 maskFirst, RectMask64 maskSecond)
            {
                return maskSecond.MaybeEncompassing(maskFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask32 maskFirst, RectMask32 maskSecond)
            {
                return maskSecond.MaybeEncompassing(maskFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask16 maskFirst, RectMask16 maskSecond)
            {
                return maskSecond.MaybeEncompassing(maskFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask8 maskFirst, RectMask8 maskSecond)
            {
                return maskSecond.MaybeEncompassing(maskFirst);
            }
        }

        /// <summary>
        /// <see cref="EncompassedByNT"/> tests whether the first rectangle is encompassed by the second (the outer). 
        /// The trivial case is disallowed. 
        /// </summary>
        public struct EncompassedByNT
            : IRectMaskRelation<EncompassedByNT, RectMask128>
            , IRectMaskRelation<EncompassedByNT, RectMask64>
            , IRectMaskRelation<EncompassedByNT, RectMask32>
            , IRectMaskRelation<EncompassedByNT, RectMask16>
            , IRectMaskRelation<EncompassedByNT, RectMask8>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Test(Rect rectFirst, Rect rectSecond)
            {
                if (rectFirst.Width <= 0 || rectFirst.Height <= 0 ||
                    rectSecond.Width <= 0 || rectSecond.Height <= 0)
                {
                    // ======
                    // The triviality case, rejected.
                    // See Encompassing for the implementation that accepts the triviality case.
                    // (This explicit check is needed because ContainsWithin will throw when 
                    // either rect is non-positive.)
                    // ------
                    return false;
                }
                return InternalRectUtility.Inline.ContainsWithin(rectOuter: rectSecond, rectInner: rectFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask128 maskFirst, RectMask128 maskSecond)
            {
                // ======
                // See class comment for proper argument ordering.
                // ------
                return maskSecond.MaybeEncompassingNT(maskFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask64 maskFirst, RectMask64 maskSecond)
            {
                return maskSecond.MaybeEncompassingNT(maskFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask32 maskFirst, RectMask32 maskSecond)
            {
                return maskSecond.MaybeEncompassingNT(maskFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask16 maskFirst, RectMask16 maskSecond)
            {
                return maskSecond.MaybeEncompassingNT(maskFirst);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TestMaybe(RectMask8 maskFirst, RectMask8 maskSecond)
            {
                return maskSecond.MaybeEncompassingNT(maskFirst);
            }
        }
    }
}
