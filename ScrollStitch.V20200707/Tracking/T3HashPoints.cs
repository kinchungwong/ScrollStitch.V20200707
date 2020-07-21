using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// Three-image trajectory (T3) hash points.
    /// </summary>
    public class T3HashPoints
    {
        /// <summary>
        /// The three image keys (image identifiers; integer-valued).
        /// </summary>
        /// 
        /// <example>
        /// 
        /// <para>
        /// To get the three image keys as an array, use either:
        /// <br/>
        /// <c>ImageKeys.ToArray()</c>, or equivalently,
        /// <br/>
        /// <c>new int[3] { ImageKeys.ItemAt(0), ImageKeys.ItemAt(1), ImageKeys.ItemAt(2) }</c>
        /// </para>
        /// 
        /// <para>
        /// Given an <c>imageKey</c>, its position on the array of image keys can be looked up as:
        /// <br/>
        /// <c>ImageKeys.IndexOf(imageKey)</c>
        /// </para>
        /// 
        /// </example>
        /// 
        public UniqueList<int> ImageKeys { get; }

        /// <summary>
        /// The list of hash values.
        /// 
        /// <para>
        /// Each of the hash value occurs once and exactly once on each of the three images.
        /// </para>
        /// </summary>
        public IReadOnlyList<int> HashValues { get; }

        /// <summary>
        /// The list of image points on each of the three images that correspond to each hash value.
        /// 
        /// <para>
        /// There are three point lists, corresponding to the three image keys. <br/>
        /// Each point list has the same length as <see cref="HashValues"/>.
        /// </para>
        /// 
        /// </summary>
        /// 
        /// <example>
        /// 
        /// <para>
        /// To iterate through all hash values and image points,
        /// <code>
        /// L001    int imageKey0 = ImageKeys.ItemAt(0); <br/>
        /// L002    int imageKey1 = ImageKeys.ItemAt(1); <br/>
        /// L003    int imageKey2 = ImageKeys.ItemAt(2); <br/>
        /// L004    var pts0 = Points[imageKey0]; <br/>
        /// L005    var pts1 = Points[imageKey1]; <br/>
        /// L006    var pts2 = Points[imageKey2]; <br/>
        /// L007    int count = HashValues.Count; <br/>
        /// L008    for (int pointIndex = 0; pointIndex &lt; count; ++pointIndex) <br/>
        /// L009    { <br/>
        /// L010        int hashValue = HashValues[pointIndex]; <br/>
        /// L011        Point p0 = pts0[pointIndex]; <br/>
        /// L012        Point p1 = pts1[pointIndex]; <br/>
        /// L013        Point p2 = pts2[pointIndex]; <br/>
        /// L014        /// Process the points <br/>
        /// L015    }
        /// </code>
        /// </para>
        /// 
        /// </example>
        /// 
        public IReadOnlyDictionary<int, IReadOnlyList<Point>> Points { get; }

        /// <summary>
        /// Constructor.
        /// 
        /// <para>
        /// It is more convenient to use the <see cref="Factory"/> class for initialization.
        /// </para>
        /// </summary>
        /// <param name="imageKeys"></param>
        /// <param name="hashValues"></param>
        /// <param name="points"></param>
        public T3HashPoints(
            IReadOnlyList<int> imageKeys,
            IReadOnlyList<int> hashValues, 
            IReadOnlyDictionary<int, IReadOnlyList<Point>> points)
        {
            ImageKeys = (imageKeys as UniqueList<int>) ?? (new UniqueList<int>(imageKeys));
            HashValues = hashValues;
            Points = points;
        }

        /// <summary>
        /// A factory class for computing <see cref="T3HashPoints"/>.
        /// </summary>
        public class Factory
        {
            private ImageManager _imageManager;
            private UniqueList<int> _imageKeys;
            private int _imageKey0;
            private int _imageKey1;
            private int _imageKey2;
            private Dictionary<int, Point> _dict0;
            private Dictionary<int, Point> _dict1;
            private Dictionary<int, Point> _dict2;
            IReadOnlyList<int> _hashValues;
            IReadOnlyDictionary<int, IReadOnlyList<Point>> _points;

            /// <summary>
            /// Creates an instance of <see cref="T3HashPoints"/> from hash points sampled from three images.
            /// </summary>
            /// <param name="imageManager"></param>
            /// <param name="imageKeys"></param>
            /// <returns></returns>
            public static T3HashPoints Create(ImageManager imageManager, IReadOnlyList<int> imageKeys)
            {
                return new Factory(imageManager, imageKeys)._Create();
            }

            private Factory(ImageManager imageManager, IReadOnlyList<int> imageKeys)
            {
                _imageManager = imageManager;
                _imageKeys = (imageKeys as UniqueList<int>) ?? (new UniqueList<int>(imageKeys));
                _imageKey0 = _imageKeys.ItemAt(0);
                _imageKey1 = _imageKeys.ItemAt(1);
                _imageKey2 = _imageKeys.ItemAt(2);
            }

            private T3HashPoints _Create()
            {
                _dict0 = _LoadPointsAsDict(_imageKey0);
                _dict1 = _LoadPointsAsDict(_imageKey1);
                _dict2 = _LoadPointsAsDict(_imageKey2);
                _PopulateLists();
                var t3hp = new T3HashPoints(_imageKeys, _hashValues, _points);
                _Cleanup();
                return t3hp;
            }

            private Dictionary<int, Point> _LoadPointsAsDict(int imageKey)
            {
                var dict = new Dictionary<int, Point>();
                var hps = _imageManager.InputHashPoints.Get(imageKey);
                foreach (var hp in hps.HashPoints)
                {
                    dict.Add(hp.HashValue, hp.Point);
                }
                return dict;
            }

            private void _PopulateLists()
            {
                var hashValues = new List<int>();
                var filtPts0 = new List<Point>();
                var filtPts1 = new List<Point>();
                var filtPts2 = new List<Point>();
                foreach (var kvp0 in _dict0)
                {
                    int hashValue = kvp0.Key;
                    Point p0 = kvp0.Value;
                    if (!_dict1.TryGetValue(hashValue, out Point p1) ||
                        !_dict2.TryGetValue(hashValue, out Point p2))
                    {
                        continue;
                    }
                    hashValues.Add(hashValue);
                    filtPts0.Add(p0);
                    filtPts1.Add(p1);
                    filtPts2.Add(p2);
                }
                _hashValues = new List<int>(hashValues).AsReadOnly();
                var dictRoPoints = new Dictionary<int, IReadOnlyList<Point>>();
                dictRoPoints.Add(_imageKey0, new List<Point>(filtPts0).AsReadOnly());
                dictRoPoints.Add(_imageKey1, new List<Point>(filtPts1).AsReadOnly());
                dictRoPoints.Add(_imageKey2, new List<Point>(filtPts2).AsReadOnly());
                _points = new ReadOnlyDictionary<int, IReadOnlyList<Point>>(dictRoPoints);
            }

            private void _Cleanup()
            {
                _imageManager = null;
                _imageKeys = null;
                _dict0 = null;
                _dict1 = null;
                _dict2 = null;
                _hashValues = null;
                _points = null;
            }
        }
    }
}
