using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// <see cref="FastRectListMethods"/> is an extension methods class for trivial wrappers over 
    /// member methods defined on <see cref="FastRectList"/>.
    /// </summary>
    public static class FastRectListMethods
    {
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
        public static int FindFirstIdentical(this FastRectList fastRectList, Rect queryRect)
        {
            return fastRectList.FindFirst(default(RectRelations.IdenticalNT), queryRect);
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
        public static int FindFirstIntersect(this FastRectList fastRectList, Rect queryRect)
        {
            return fastRectList.FindFirst(default(RectRelations.Intersect), queryRect);
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
        public static int FindFirstEncompassing(this FastRectList fastRectList, Rect queryRect)
        {
            return fastRectList.FindFirst(default(RectRelations.EncompassingNT), queryRect);
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
        public static int FindFirstEncompassedBy(this FastRectList fastRectList, Rect queryRect)
        {
            return fastRectList.FindFirst(default(RectRelations.EncompassedByNT), queryRect);
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
        public static IEnumerable<int> EnumerateIdentical(this FastRectList fastRectList, Rect queryRect)
        {
            return fastRectList.Enumerate(default(RectRelations.IdenticalNT), queryRect);
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
        public static IEnumerable<int> EnumerateIntersect(this FastRectList fastRectList, Rect queryRect)
        {
            return fastRectList.Enumerate(default(RectRelations.Intersect), queryRect);
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
        public static IEnumerable<int> EnumerateEncompassing(this FastRectList fastRectList, Rect queryRect)
        {
            return fastRectList.Enumerate(default(RectRelations.EncompassingNT), queryRect);
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
        public static IEnumerable<int> EnumerateEncompassedBy(this FastRectList fastRectList, Rect queryRect)
        {
            return fastRectList.Enumerate(default(RectRelations.EncompassedByNT), queryRect);
        }

        /// <summary>
        /// Calls the specified function for each rectangle item that intersects with the query rectangle.
        /// 
        /// <para>
        /// There are several <c>ForEach</c> overloads: <br/>
        /// <see cref="ForEach(FastRectList, Rect, Action{Rect})"/> <br/>
        /// <see cref="ForEach(FastRectList, Rect, Action{int, Rect})"/> <br/>
        /// <see cref="ForEach(FastRectList, Rect, Action{int})"/> <br/>
        /// <see cref="ForEach(FastRectList, Rect, Func{Rect, bool})"/> <br/>
        /// <see cref="ForEach(FastRectList, Rect, Func{int, Rect, bool})"/> <br/>
        /// <see cref="ForEach(FastRectList, Rect, Func{int, bool})"/>
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
        public static void ForEach(this FastRectList fastRectList, Rect queryRect, Func<int, Rect, bool> func)
        {
            fastRectList.ForEach(default(RectRelations.Intersect), queryRect, 
                new FastRectList.HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(FastRectList, Rect, Func{int, Rect, bool})"/>
        public static void ForEach(this FastRectList fastRectList, Rect queryRect, Func<int, bool> func)
        {
            fastRectList.ForEach(default(RectRelations.Intersect), queryRect, 
                new FastRectList.HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(FastRectList, Rect, Func{int, Rect, bool})"/>
        public static void ForEach(this FastRectList fastRectList, Rect queryRect, Func<Rect, bool> func)
        {
            fastRectList.ForEach(default(RectRelations.Intersect), queryRect, 
                new FastRectList.HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(FastRectList, Rect, Func{int, Rect, bool})"/>
        public static void ForEach(this FastRectList fastRectList, Rect queryRect, Action<int, Rect> func)
        {
            fastRectList.ForEach(default(RectRelations.Intersect), queryRect, 
                new FastRectList.HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(FastRectList, Rect, Func{int, Rect, bool})"/>
        public static void ForEach(this FastRectList fastRectList, Rect queryRect, Action<int> func)
        {
            fastRectList.ForEach(default(RectRelations.Intersect), queryRect, 
                new FastRectList.HelperClasses.FuncAdapter(func));
        }

        /// <inheritdoc cref="ForEach(FastRectList, Rect, Func{int, Rect, bool})"/>
        public static void ForEach(this FastRectList fastRectList, Rect queryRect, Action<Rect> func)
        {
            fastRectList.ForEach(default(RectRelations.Intersect), queryRect, 
                new FastRectList.HelperClasses.FuncAdapter(func));
        }
    }
}
