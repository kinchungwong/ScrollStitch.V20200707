using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Collections.Specialized;
    using ScrollStitch.V20200707.Data;
    using System.Collections;
    using System.Runtime.CompilerServices;

    public class T3Movements
    {
        /// <summary>
        /// <inheritdoc cref="T3HashPoints"/>
        /// </summary>
        public T3HashPoints HashPoints { get; }

        /// <summary>
        /// <inheritdoc cref="T3HashPoints.ImageKeys"/>
        /// </summary>
        public UniqueList<int> ImageKeys 
            => HashPoints.ImageKeys;

        /// <summary>
        /// <inheritdoc cref="T3HashPoints.HashValues"/>
        /// </summary>
        public IReadOnlyList<int> HashValues 
            => HashPoints.HashValues;

        /// <summary>
        /// <inheritdoc cref="T3HashPoints.Points"/>
        /// </summary>
        public IReadOnlyDictionary<int, IReadOnlyList<Point>> Points 
            => HashPoints.Points;

        /// <summary>
        /// The list of three-image movement tuples.
        /// 
        /// <example>
        /// <para>
        /// To look up the trajectory identifier given a three-image movement tuple, use the following:
        /// <br/>
        /// <code>
        /// L001    Point p0, p1, p2; <br/>
        /// L002    Movement m01 = p1 - p0; <br/>
        /// L003    Movement m12 = p2 - p1; <br/>
        /// L004    int trajectoryId = Movements.IndexOf((m01, m12)); <br/>
        /// </code><br/>
        /// </para>
        /// 
        /// <para>
        /// To look up the three-image movement tuple by its integer identifier, use the following:
        /// <br/>
        /// <code>
        /// L001    int trajectoryId; <br/>
        /// L002    (Movement m01, Movement m12) = Movements.ItemAt(trajectoryId); <br/>
        /// </code><br/>
        /// </para>
        /// </example>
        /// 
        /// </summary>
        /// 
        public UniqueList<(Movement, Movement)> Movements { get; }

        /// <summary>
        /// The trajectory identifiers assigned to each triplet of hash points.
        /// 
        /// <para>
        /// The trajectory identifier is the index position of the three-image movement tuple found in <see cref="Movements"/>.
        /// <br/>
        /// This list has the same length as <see cref="HashValues"/>, as well as the three lists of points from <see cref="Points"/>.
        /// </para>
        /// </summary>
        /// 
        public IReadOnlyList<int> Labels { get; }

        /// <summary>
        /// The histogram of the number of hash points assigned to each trajectory identifier.
        /// 
        /// <para>
        /// As a reminder, a trajectory identifier ("label") is uniquely associated with a three-image movement tuple.
        /// </para>
        /// </summary>
        //public IHistogram<(Movement, Movement), int> MovementPointCounts { get; }
        public IReadOnlyDictionary<int, int> LabelPointCounts { get; }

        /// <summary>
        /// Constructor.
        /// 
        /// <para>
        /// It is more convenient to use the <see cref="Factory"/> class for initialization.
        /// </para>
        /// </summary>
        /// 
        /// <param name="hashPoints"></param>
        /// <param name="movements"></param>
        /// <param name="labels"></param>
        /// 
        public T3Movements(
            T3HashPoints hashPoints, 
            UniqueList<(Movement, Movement)> movements, 
            IReadOnlyList<int> labels,
            IReadOnlyDictionary<int, int> labelPointCounts)
        {
            HashPoints = hashPoints;
            Movements = movements;
            Labels = labels;
            LabelPointCounts = labelPointCounts;
        }

        /// <summary>
        /// Factory class for T3MovementLabels
        /// </summary>
        public class Factory
        {
            private T3HashPoints _input;
            private UniqueList<int> _imageKeys;
            private int _imageKey0;
            private int _imageKey1;
            private int _imageKey2;
            private IReadOnlyList<Point> _pts0;
            private IReadOnlyList<Point> _pts1;
            private IReadOnlyList<Point> _pts2;
            private UniqueList<(Movement, Movement)> _movements;
            private IReadOnlyList<int> _labels;
            private IReadOnlyDictionary<int, int> _labelPointCounts;

            /// <summary>
            /// Creates an instance of <see cref="T3Movements"/> by analyzing the relative movements 
            /// of each triplet of hash points in the <paramref name="input"/>.
            /// </summary>
            /// <param name="input"></param>
            /// <returns></returns>
            public static T3Movements Create(T3HashPoints input)
            {
                return new Factory(input)._Create();
            }

            private Factory(T3HashPoints input)
            {
                _input = input;
                _imageKeys = input.ImageKeys;
                _imageKey0 = _imageKeys.ItemAt(0);
                _imageKey1 = _imageKeys.ItemAt(1);
                _imageKey2 = _imageKeys.ItemAt(2);
                _pts0 = input.Points[_imageKey0];
                _pts1 = input.Points[_imageKey1];
                _pts2 = input.Points[_imageKey2];
            }

            private T3Movements _Create()
            {
                _ProcessHashPoints();
                var t3m = new T3Movements(_input, _movements, _labels, _labelPointCounts);
                _Cleanup();
                return t3m;
            }

            private void _ProcessHashPoints()
            {
                int count = _pts0.Count;
                var labels = new List<int>(capacity: count);
                _movements = new UniqueList<(Movement, Movement)>();
                var labelHist = HistogramUtility.CreateIntHistogram<int>();
                if (_pts1.Count != count ||
                    _pts2.Count != count)
                {
                    throw new Exception();
                }
                for (int index = 0; index < count; ++index)
                {
                    Point p0 = _pts0[index];
                    Point p1 = _pts1[index];
                    Point p2 = _pts2[index];
                    Movement m01 = p1 - p0;
                    Movement m12 = p2 - p1;
                    var m012 = (m01, m12);
                    int label = _movements.Add(m012);
                    labels.Add(label);
                    labelHist.Add(label);
                }
                _labels = labels.AsReadOnly();
                _labelPointCounts = labelHist.ToDictionary().AsReadOnly();
            }

            private void _Cleanup()
            {
                _input = null;
                _imageKeys = null;
                _pts0 = null;
                _pts1 = null;
                _pts2 = null;
                _movements = null;
                _labels = null;
                _labelPointCounts = null;
            }
        }
    }
}
