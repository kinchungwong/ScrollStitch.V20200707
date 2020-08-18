using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Data;

    public class FastRectPyramid
    {
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

            public const double DefaultRatio = 32.0;

            /// <summary>
            /// Maximum number of levels that this class will create.
            /// </summary>
            public const int MaxLevelsCreated = 64;
        }

        public static class Builder
        {
            public static FastRectPyramid Create()
            {
                return Create(
                    minRadius: Constants.MinSupportedRadius,
                    maxRadius: Constants.MaxSupportedRadius,
                    ratio: Constants.DefaultRatio);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="minRadius"></param>
            /// <param name="maxRadius"></param>
            /// 
            /// <param name="ratio">
            /// The valid range of ratio is from <see cref="MinRatio"/> to <see cref="MaxRatio"/>, 
            /// both inclusive.
            /// </param>
            /// 
            public static FastRectPyramid Create(int minRadius, int maxRadius, double ratio)
            {
                List<int> radiusList = InitRadiusList(minRadius, maxRadius, ratio);
                var frp = new FastRectPyramid(radiusList);
                return frp;
            }

            public static List<int> InitRadiusList(int innermostRadius, int outermostRadius, double ratio)
            {
                if (innermostRadius < Constants.MinSupportedRadius ||
                    innermostRadius > Constants.MaxSupportedRadius)
                {
                    throw new ArgumentOutOfRangeException(nameof(innermostRadius));
                }
                if (outermostRadius < innermostRadius ||
                    outermostRadius > Constants.MaxSupportedRadius)
                {
                    throw new ArgumentOutOfRangeException(nameof(outermostRadius));
                }
                if (!(ratio >= Constants.MinRatio &&
                    ratio <= Constants.MaxRatio))
                {
                    throw new ArgumentOutOfRangeException(nameof(ratio));
                }
                var radiusList = new List<int>();
                if (outermostRadius == innermostRadius)
                {
                    radiusList.Add(outermostRadius);
                    return radiusList;
                }
                double ratioFloor = (innermostRadius + 1.0) / (innermostRadius);
                ratio = Math.Max(ratio, ratioFloor);
                double logRadiusRatio = Math.Log(outermostRadius) - Math.Log(innermostRadius);
                double logRatio = Math.Log(ratio);
                if (logRatio * (Constants.MaxLevelsCreated - 1) < logRadiusRatio)
                {
                    logRatio = logRadiusRatio / (Constants.MaxLevelsCreated - 1);
                }
                double approxStepCount = logRadiusRatio / logRatio;
                int roundedStepCount = (int)Math.Round(approxStepCount);
                if (roundedStepCount < 1)
                {
                    radiusList.Add(innermostRadius);
                    radiusList.Add(outermostRadius);
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
                    double approxRadius = innermostRadius * Math.Exp(logRadiusRatio * level / roundedStepCount);
                    int roundedRadius = (int)Math.Round(approxRadius);
                    radiusList.Add(roundedRadius);
                }
                return radiusList;
            }
        }

        public IReadOnlyList<int> RadiusList => _childRadiusList.AsReadOnly();

        #region private
        private List<int> _childRadiusList;
        private List<Rect> _childRects;
        #endregion

        public FastRectPyramid(List<int> childRadiusList)
        {
            int levelCount = childRadiusList?.Count ?? 0;
            if (levelCount < 1)
            {
                throw new ArgumentException(nameof(childRadiusList));
            }
            _childRadiusList = childRadiusList;
            _CtorValidateRadiusList();
            _CtorInitRectList();
        }

        private void _CtorValidateRadiusList()
        {
            int count = _childRadiusList.Count;
            for (int index = 0; index < count; ++index)
            {
                int radius = _childRadiusList[index];
                if (radius < Constants.MinSupportedRadius ||
                    radius > Constants.MaxSupportedRadius)
                {
                    throw new InvalidOperationException();
                }
                if (index >= 1)
                { 
                    int prevRadius = _childRadiusList[index - 1];
                    if (radius <= prevRadius)
                    {
                        throw new InvalidOperationException();
                    }
                }
            }
        }

        private void _CtorInitRectList()
        {
            int count = _childRadiusList.Count;
            _childRects = new List<Rect>(capacity: count);
            for (int level = 0; level < count; ++level)
            {
                int levelRadius = _childRadiusList[level];
                int levelLength = 2 * levelRadius;
                var rect = new Rect(-levelRadius, -levelRadius, levelLength, levelLength);
                _childRects.Add(rect);
            }
        }
    }
}
