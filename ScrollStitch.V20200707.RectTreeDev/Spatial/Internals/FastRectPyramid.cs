using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// <see cref="FastRectPyramid"/> is a static non-generic class which contains static non-generic members
    /// used by instances of the generic <see cref="FastRectPyramid{T}"/>.
    /// 
    /// <para>
    /// To create an instance of <see cref="FastRectPyramid{T}"/>, use either 
    /// </para>
    /// </summary>
    public static class FastRectPyramid
    {
        /// <summary>
        /// Parameter limits and initialization constants for <see cref="FastRectPyramid{T}"/>.
        /// </summary>
        public static class Constants
        {
            /// <summary>
            /// Minimum supported radius.
            /// </summary>
            public const int MinSupportedRadius = 32;

            /// <summary>
            /// Maximum supported radius.
            /// 
            /// <para>
            /// This value is chosen to be the maximum allowed value for the four parameters 
            /// of a <see cref="Rect"/>, namely <see cref="Rect.X"/>, <see cref="Rect.Y"/>, 
            /// <see cref="Rect.Width"/>, and <see cref="Rect.Height"/>.
            /// </para>
            /// </summary>
            public const int MaxSupportedRadius = (1024 * 1024 * 1024) - 1;

            /// <summary>
            /// Maximum allowed value for ratio.
            /// </summary>
            public const double MaxRatio = 64.0;

            /// <summary>
            /// The minimum allowed value for ratio. That is, this is the minimum ratio between the lengths 
            /// of any two adjacent child bounding rectangles.
            /// 
            /// <para>
            /// This value, approximately <c>1.059463</c>, is chosen to be slightly below: 
            /// <c>Math.Pow(2.0, (1.0 / 12.0))</c>, which is also known as the semitone ratio.
            /// </para>
            /// 
            /// <para>
            /// A ratio that is too close to <c>1.0</c> will make it problematic or impossible to enumerate
            /// the pyramid levels. <br/>
            /// Consider a <c>minRadius</c> of 256 and a <c>maxRadius</c> of 1024. If a ratio of <c>1.0</c>
            /// is specified, enumeration will obviously fail, as one can never get to 1024 just by repeatedly 
            /// applying the ratio <c>1.0</c> on the starting value of 256.
            /// </para>
            /// </summary>
            public const double MinRatio = 1.059463;

            /// <summary>
            /// <see cref="DefaultRatio"/> is chosen to be <c>32.0</c> for a very steep and aggressive scaling
            /// of pyramid child rectangle sizes, in order to cover a vast space up to 
            /// <see cref="MaxSupportedRadius"/>.
            /// </summary>
            public const double DefaultRatio = 32.0;

            /// <summary>
            /// Maximum number of levels that this class will create.
            /// </summary>
            public const int MaxLevelsCreated = 64;
        }

        /// <summary>
        /// The static builder for <see cref="FastRectPyramid{T}"/>.
        /// 
        /// <para>
        /// One could access either <see cref="Builder"/> or 
        /// <see cref="FastRectPyramid{T}.Builder"/>, with only minor difference in how the generic 
        /// parameter is specified.
        /// </para>
        /// 
        /// </summary>
        public static class Builder
        {
            /// <summary>
            /// Creates an instance of <see cref="FastRectPyramid{T}"/> with data type <see cref="T"/>
            /// and with a pyramid of child rectangles large enough to cover the entire valid range of
            /// sizes of <see cref="Rect"/>.
            /// 
            /// <para>
            /// It is equivalent to calling <see cref="Create{T}(int, int, double)"/> with the following:
            /// <br/>
            /// ... <c>minRadius: <see cref="Constants.MinSupportedRadius"/></c> <br/>
            /// ... <c>maxRadius: <see cref="Constants.MaxSupportedRadius"/></c> <br/>
            /// ... <c>ratio: <see cref="Constants.DefaultRatio"/></c>
            /// </para>
            /// </summary>
            /// 
            /// <typeparam name="T">
            /// The data type associated with each item.
            /// </typeparam>
            /// 
            /// <returns>
            /// A default-initialized instance of <see cref="FastRectPyramid{T}"/>.
            /// </returns>
            /// 
            public static FastRectPyramid<T> Create<T>()
            {
                return new FastRectPyramid<T>(InitDefaultRadiusList());
            }

            /// <summary>
            /// Creates an instance of <see cref="FastRectPyramid{T}"/> with data type <see cref="T"/>
            /// and with a pyramid of child rectangles 
            /// </summary>
            /// 
            /// <param name="minRadius">
            /// The radius (half-width and half-height) of the smallest rectangle in the pyramid.
            /// <br/>
            /// This value cannot be below <see cref="Constants.MinSupportedRadius"/>.
            /// </param>
            /// 
            /// <param name="maxRadius">
            /// The radius (half-width and half-height) of the largest rectangle in the pyramid.
            /// <br/>
            /// This value must be above <paramref name="minRadius"/>, and cannot exceed
            /// <see cref="Constants.MaxSupportedRadius"/>.
            /// </param>
            /// 
            /// <param name="ratio">
            /// The ratio between the lengths (widths and heights) of two adjacent levels of the pyramid.
            /// <br/>
            /// This value cannot be below <see cref="Constants.MinRatio"/> or above 
            /// <see cref="Constants.MaxRatio"/>.
            /// </param>
            /// 
            /// <typeparam name="T">
            /// The data type associated with each item.
            /// </typeparam>
            /// 
            /// <returns>
            /// An instance of <see cref="FastRectPyramid{T}"/>.
            /// </returns>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">
            /// One or more of the parameters are outside the valid range. Refer to 
            /// <see cref="Create{T}(int, int, double)"/> for a description of their valid ranges.
            /// </exception>
            /// 
            public static FastRectPyramid<T> Create<T>(int minRadius, int maxRadius, double ratio)
            {
                List<int> radiusList = InitRadiusList(minRadius, maxRadius, ratio);
                var frp = new FastRectPyramid<T>(radiusList);
                return frp;
            }

            /// <summary>
            /// <para>
            /// It is equivalent to calling <see cref="InitRadiusList(int, int, double)"/> with the following:
            /// <br/>
            /// ... <c>minRadius: <see cref="Constants.MinSupportedRadius"/></c> <br/>
            /// ... <c>maxRadius: <see cref="Constants.MaxSupportedRadius"/></c> <br/>
            /// ... <c>ratio: <see cref="Constants.DefaultRatio"/></c>
            /// </para>
            /// </summary>
            /// 
            public static List<int> InitDefaultRadiusList()
            { 
                return InitRadiusList(
                    minRadius: Constants.MinSupportedRadius,
                    maxRadius: Constants.MaxSupportedRadius,
                    ratio: Constants.DefaultRatio);
            }

            /// <summary>
            /// Initializes the list of radii (half-widths and half-heights) for a pyramid of 
            /// child rectangles given the parameters.
            /// </summary>
            /// 
            /// <param name="minRadius">
            /// The radius (half-width and half-height) of the smallest rectangle in the pyramid.
            /// <br/>
            /// This value cannot be below <see cref="Constants.MinSupportedRadius"/>.
            /// </param>
            /// 
            /// <param name="maxRadius">
            /// The radius (half-width and half-height) of the largest rectangle in the pyramid.
            /// <br/>
            /// This value must be above <paramref name="minRadius"/>, and cannot exceed
            /// <see cref="Constants.MaxSupportedRadius"/>.
            /// </param>
            /// 
            /// <param name="ratio">
            /// The ratio between the lengths (widths and heights) of two adjacent levels of the pyramid.
            /// <br/>
            /// This value cannot be below <see cref="Constants.MinRatio"/> or above 
            /// <see cref="Constants.MaxRatio"/>.
            /// </param>
            /// 
            /// <returns>
            /// A list of radii for a pyramid of child rectangles that is suitable for initializing an 
            /// instance of <see cref="FastRectPyramid{T}"/>.
            /// </returns>
            /// 
            /// <exception cref="ArgumentOutOfRangeException">
            /// One or more of the parameters are outside the valid range. Refer to 
            /// <see cref="Create{T}(int, int, double)"/> for a description of their valid ranges.
            /// </exception>
            /// 
            public static List<int> InitRadiusList(int minRadius, int maxRadius, double ratio)
            {
                if (minRadius < Constants.MinSupportedRadius ||
                    minRadius > Constants.MaxSupportedRadius)
                {
                    throw new ArgumentOutOfRangeException(nameof(minRadius));
                }
                if (maxRadius < minRadius ||
                    maxRadius > Constants.MaxSupportedRadius)
                {
                    throw new ArgumentOutOfRangeException(nameof(maxRadius));
                }
                if (!(ratio >= Constants.MinRatio &&
                    ratio <= Constants.MaxRatio))
                {
                    throw new ArgumentOutOfRangeException(nameof(ratio));
                }
                var radiusList = new List<int>();
                if (maxRadius == minRadius)
                {
                    radiusList.Add(maxRadius);
                    return radiusList;
                }
                double ratioFloor = (minRadius + 1.0) / (minRadius);
                ratio = Math.Max(ratio, ratioFloor);
                double logRadiusRatio = Math.Log(maxRadius) - Math.Log(minRadius);
                double logRatio = Math.Log(ratio);
                if (logRatio * (Constants.MaxLevelsCreated - 1) < logRadiusRatio)
                {
                    logRatio = logRadiusRatio / (Constants.MaxLevelsCreated - 1);
                }
                double approxStepCount = logRadiusRatio / logRatio;
                int roundedStepCount = (int)Math.Round(approxStepCount);
                if (roundedStepCount < 1)
                {
                    radiusList.Add(minRadius);
                    radiusList.Add(maxRadius);
                    return radiusList;
                }
                int levelCount = roundedStepCount + 1;
                if (levelCount > Constants.MaxLevelsCreated)
                {
                    // impossible; unexpected.
                    throw new Exception();
                }
                for (int level = 0; level < levelCount; ++level)
                {
                    double approxRadius = minRadius * Math.Exp(logRadiusRatio * level / roundedStepCount);
                    int roundedRadius = (int)Math.Round(approxRadius);
                    radiusList.Add(roundedRadius);
                }
                return radiusList;
            }
        }
    }

    /// <summary>
    /// <see cref="FastRectPyramid{T}"/> is designed to be a root node of a searchable rectangular data structure.
    /// 
    /// <para>
    /// Unlike other rectangular search trees that require a predefined bounding rectangle at every node,
    /// this class does not need to be initialized with one.
    /// </para>
    /// 
    /// <para>
    /// Instead, the <see cref="Builder.Create"/> method returns an instance of this class with a pyramid of 
    /// rectangles, all "concentric" (centered at <c>Rect(-1, -1, 2, 2)</c>) and partially overlapping. The sizes 
    /// of these rectangles form a geometric series. Thus, the number of pyramid levels is logarithmic of the
    /// largest rectangular bound needed to handle all foreseeable items.
    /// </para>
    /// 
    /// <para>
    /// Each rectangle in this pyramid is associted with an instance of a secondary searchable rectangular data 
    /// structure, which may be tree-based or list-based. Rectangles added as items to this class are sent to
    /// the child instance having the smallest bounding rectangle that encompasses the item rectangle.
    /// </para>
    /// 
    /// <para>
    /// To create an instance of this class, use <see cref="Builder"/> instead of the constructor.
    /// </para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FastRectPyramid<T>
        : IRectQuery<KeyValuePair<Rect, T>>
        , ICollection<KeyValuePair<Rect, T>>
        , IReadOnlyCollection<KeyValuePair<Rect, T>>
    {
        /// <summary>
        /// The static builder for <see cref="FastRectPyramid{T}"/>.
        /// 
        /// <para>
        /// One could access either <see cref="FastRectPyramid.Builder"/> or 
        /// <see cref="Builder"/>, with only minor difference in how the generic 
        /// parameter is specified.
        /// </para>
        /// </summary>
        /// 
        public static class Builder
        {
            /// <inheritdoc cref="FastRectPyramid.Builder.Create{T}()"/>
            ///
            public static FastRectPyramid<T> Create()
            {
                return FastRectPyramid.Builder.Create<T>();
            }

            /// <inheritdoc cref="FastRectPyramid.Builder.Create{T}(int, int, double)"/>
            ///
            public static FastRectPyramid<T> Create(int minRadius, int maxRadius, double ratio)
            {
                return FastRectPyramid.Builder.Create<T>(
                    minRadius: minRadius,
                    maxRadius: maxRadius,
                    ratio: ratio);
            }
        }

        public IReadOnlyList<int> RadiusList => _childRadiusList.AsReadOnly();

        public int Count { get; private set; }

        public bool IsReadOnly => false;

        public Func<Rect, ICollection<KeyValuePair<Rect, T>>> ChildCollectionCreateFunc;

        #region private
        private List<int> _childRadiusList;
        private List<Rect> _childRects;
        private List<ICollection<KeyValuePair<Rect, T>>> _childCollections;
        #endregion

        public FastRectPyramid(IList<int> childRadiusList)
        {
            _CtorValidateRadiusList(childRadiusList);
            _CtorInitChildLists(childRadiusList);
#if false
            ChildCollectionCreateFunc = (Rect childRect) => new FastRectNode<T>(childRect, new FastRectNodeSettings());
#elif true
            ChildCollectionCreateFunc = (Rect childRect) => new FastRectDataList<T>(childRect);
#endif
        }

        public void Add(Rect rect, T data)
        {
            if (rect.Width <= 0 ||
                rect.Height <= 0)
            {
                throw new ArgumentException(nameof(rect));
            }
            int levelIndex = _GetLevelForRect(rect);
            _AddToLevel(levelIndex, rect, data);
            ++Count;
        }

        public void Add(KeyValuePair<Rect, T> item)
        {
            Add(item.Key, item.Value);
        }

        public IEnumerable<KeyValuePair<Rect, T>> Enumerate(Rect queryRect)
        {
            int queryLevel = _GetLevelForRect(queryRect);
            for (int childLevel = 0; childLevel <= queryLevel; ++childLevel)
            {
                var childBounds = _childRects[childLevel];
                if (!InternalRectUtility.Inline.HasIntersect(childBounds, queryRect))
                {
                    continue;
                }
                var child = _childCollections[childLevel];
                if (child is null)
                {
                    continue;
                }
                // ======
                // Starting C# language version 5.0, the "foreach yield return" can be replaced 
                // with this one-liner:
                // ------
                // yield foreach (child as something).Enumerate(queryRect);
                // ------
                switch (child)
                {
                    case IRectQuery<KeyValuePair<Rect, T>> rectQuery:
                        foreach (var itemRectData in rectQuery.Enumerate(queryRect))
                        {
                            yield return itemRectData;
                        }
                        break;

                    default:
                        foreach (var itemRectData in child)
                        {
                            yield return itemRectData;
                        }
                        break;
                }
            }
        }

        public bool ContainsRect(Rect rect)
        {
            foreach (var itemRectData in Enumerate(rect))
            {
                var itemRect = itemRectData.Key;
                if (itemRect.Equals(rect))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(KeyValuePair<Rect, T> rectData)
        {
            var dataComparer = EqualityComparer<T>.Default;
            foreach (var itemRectData in Enumerate(rectData.Key))
            {
                var itemRect = itemRectData.Key;
                if (itemRect.Equals(rectData.Key) &&
                    dataComparer.Equals(itemRectData.Value, rectData.Value))
                {
                    return true;
                }
            }
            return false;
        }

        private int _GetLevelForRect(Rect rect)
        {
            int distMinX = _DistFromCenter(rect.Left);
            int distMinY = _DistFromCenter(rect.Top);
            int distMaxX = _DistFromCenter(rect.Right - 1);
            int distMaxY = _DistFromCenter(rect.Bottom - 1);
            int maxDist = Math.Max(Math.Max(Math.Max(distMinX, distMinY), distMaxX), distMaxY);
            // ====== Remark ======
            // The return value of BinarySearch is encoded if it is negative.
            // Refer to .NET reference for details.
            // ------
            int encodedLevelIndex = _childRadiusList.BinarySearch(maxDist);
            if (encodedLevelIndex == ~_childRadiusList.Count)
            {
                throw new InvalidOperationException("Bounds exceeded for new Rect item.");
            }
            int levelIndex = (encodedLevelIndex >= 0) ? encodedLevelIndex : ~encodedLevelIndex;
            return levelIndex;
        }

        private void _AddToLevel(int levelIndex, Rect rect, T data)
        {
            var child = _EnsureLevelCreated(levelIndex);
            child.Add(new KeyValuePair<Rect, T>(rect, data));
        }

        private ICollection<KeyValuePair<Rect, T>> _EnsureLevelCreated(int levelIndex)
        {
            if (levelIndex < 0 || levelIndex >= _childCollections.Count)
            {
                // Impossible
                throw new Exception(); 
            }
            var child = _childCollections[levelIndex];
            if (child is null)
            {
                var childRect = _childRects[levelIndex];
                child = ChildCollectionCreateFunc(childRect);
                _childCollections[levelIndex] = child;
            }
            return child;
        }

        private int _DistFromCenter(int value)
        {
            // ====== Remark ======
            // The value returned from this function is never below 1.
            // This function returns 1 when input is -1 or 0.
            // This function returns 2 when input is -2 or 1.
            // This function returns 3 when input is -3 or 2.
            // And so on.
            // ------
            // This is due to how the child rectangles are "centered".
            // For example, (0, 0), (0, -1), (-1, 0), (-1, -1) are the four pixels
            // closest to the center of each child rectangle created by this class.
            // ======
            value = (value < 0) ? -value : (value + 1);
            return value;
        }

        private static void _CtorValidateRadiusList(IList<int> childRadiusList)
        {
            int levelCount = childRadiusList?.Count ?? 0;
            if (levelCount < 1)
            {
                throw new ArgumentException(nameof(childRadiusList));
            }
            int count = childRadiusList.Count;
            for (int index = 0; index < count; ++index)
            {
                int radius = childRadiusList[index];
                if (radius < FastRectPyramid.Constants.MinSupportedRadius ||
                    radius > FastRectPyramid.Constants.MaxSupportedRadius)
                {
                    throw new InvalidOperationException();
                }
                if (index >= 1)
                { 
                    int prevRadius = childRadiusList[index - 1];
                    if (radius <= prevRadius)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        private void _CtorInitChildLists(IList<int> childRadiusList)
        {
            _childRadiusList = new List<int>(childRadiusList);
            int count = _childRadiusList.Count;
            _childRects = new List<Rect>(capacity: count);
            _childCollections = new List<ICollection<KeyValuePair<Rect, T>>>(capacity: count);
            for (int level = 0; level < count; ++level)
            {
                // ======
                // The following information is important for rectangular search and precise 
                // parameter validation purposes.
                // ------
                // The child rectangles created here have a half-pixel center at (-0.5, -0.5).
                // ------
                // For example, (0, 0), (0, -1), (-1, 0), (-1, -1) are the four pixels closest 
                // to the center of each child rectangle.
                // ======
                int levelRadius = _childRadiusList[level];
                int levelLength = 2 * levelRadius;
                var rect = new Rect(-levelRadius, -levelRadius, levelLength, levelLength);
                _childRects.Add(rect);
                // ======
                // Child secondary data structures are always created lazily - only when items
                // need to be inserted at that particular level.
                // ------
                _childCollections.Add(null);
            }
        }

        public void Clear()
        {
            Count = 0;
            foreach (var child in _childCollections)
            {
                if (child is null)
                {
                    continue;
                }
                child.Clear();
            }
        }

        public void CopyTo(KeyValuePair<Rect, T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<Rect, T> item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<Rect, T>> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
