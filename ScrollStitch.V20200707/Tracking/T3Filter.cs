using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace ScrollStitch.V20200707.Tracking
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// A class that removes three-image hash point triplets and movement tuples according to 
    /// an earlier run of <see cref="T3Classifier"/>.
    /// 
    /// <para>
    /// This class creates a new instance of <see cref="T3HashPoints"/> and <see cref="T3Movements"/>
    /// such that samples belonging to low-confidence (rejected) trajectories are removed.
    /// <br/>
    /// Such removal of samples will cause the index of triplets and trajectory labels to be 
    /// shifted. Thus, care must be taken not to confuse the old and new item index assignment.
    /// </para>
    /// </summary>
    public class T3Filter
    {
        public T3HashPoints OldHashPoints { get; }

        public T3Movements OldMovements { get; }

        public T3Classifier Classifier { get; }

        public T3HashPoints NewHashPoints { get; private set; }

        public T3Movements NewMovements { get; private set; }

        #region private
        private int _imageKey0;
        private int _imageKey1;
        private int _imageKey2;
        private UniqueList<(Movement, Movement)> _newMovementList;
        private Dictionary<int, int> _oldToNewMapping;
        private Dictionary<int, int> _newToOldMapping;
        private List<int> _newHashValues;
        private Dictionary<int, IReadOnlyList<Point>> _newPoints;
        private List<int> _newLabels;
        private Dictionary<int, int> _newLabelPointCounts;
        private int _oldPointCount;
        private int _newPointCount;
        private Dictionary<int, int> _unmatchedPointCounts;
        #endregion

        public T3Filter(T3HashPoints oldHashPoints, T3Movements oldMovements, T3Classifier classifier)
        {
            OldHashPoints = oldHashPoints;
            OldMovements = oldMovements;
            Classifier = classifier;
            _imageKey0 = oldHashPoints.ImageKeys.ItemAt(0);
            _imageKey1 = oldHashPoints.ImageKeys.ItemAt(1);
            _imageKey2 = oldHashPoints.ImageKeys.ItemAt(2);
            _oldPointCount = OldHashPoints.HashValues.Count;
        }

        public void Process()
        {
            _FilterMovementTuples();
            _FilterHashPointLabelLists();
            _DbgValidateNewPointCount();
            _CalculateUnmatchedPointCounts();
            _InitNewInstances();
        }

        private void _FilterMovementTuples()
        {
            _newMovementList = new UniqueList<(Movement, Movement)>();
            _oldToNewMapping = new Dictionary<int, int>();
            _newToOldMapping = new Dictionary<int, int>();
            int oldLabelCount = OldMovements.Movements.Count;
            var oldLabelPointCounts = OldMovements.LabelPointCounts;
            _newPointCount = 0;
            _newLabelPointCounts = new Dictionary<int, int>();
            for (int oldLabel = 0; oldLabel < oldLabelCount; ++oldLabel)
            {
                (Movement, Movement) oldMM = OldMovements.Movements.ItemAt(oldLabel);
                T3ClassifierFlags flag = Classifier.ClassifyMovement(oldMM);
                bool isAccepted = flag.HasFlag(T3ClassifierFlags.Accepted);
                bool isStationary = flag.HasFlag(T3ClassifierFlags.Stationary);
                if (!isAccepted && !isStationary)
                {
                    continue;
                }
                int newLabel = _newMovementList.Add(oldMM);
                _oldToNewMapping.Add(oldLabel, newLabel);
                _newToOldMapping.Add(newLabel, oldLabel);
                int thisLabelPointCount = oldLabelPointCounts[oldLabel];
                _newLabelPointCounts.Add(newLabel, thisLabelPointCount);
                _newPointCount += thisLabelPointCount;
            }
        }

        private void _FilterHashPointLabelLists()
        {
            var oldHashValues = OldHashPoints.HashValues;
            var oldPts0 = OldHashPoints.Points[_imageKey0];
            var oldPts1 = OldHashPoints.Points[_imageKey1];
            var oldPts2 = OldHashPoints.Points[_imageKey2];
            var oldLabels = OldMovements.Labels;
            int count = oldHashValues.Count;
            if (oldPts0.Count != count ||
                oldPts1.Count != count ||
                oldPts2.Count != count ||
                oldLabels.Count != count)
            {
                throw new Exception(); // impossible
            }
            var newHashValues = new List<int>(capacity: _newPointCount);
            var newPts0 = new List<Point>(capacity: _newPointCount);
            var newPts1 = new List<Point>(capacity: _newPointCount);
            var newPts2 = new List<Point>(capacity: _newPointCount);
            var newLabels = new List<int>(capacity: _newPointCount);
            for (int index = 0; index < count; ++index)
            {
                int oldLabel = oldLabels[index];
                if (!_oldToNewMapping.TryGetValue(oldLabel, out int newLabel))
                {
                    continue;
                }
                newHashValues.Add(oldHashValues[index]);
                newPts0.Add(oldPts0[index]);
                newPts1.Add(oldPts1[index]);
                newPts2.Add(oldPts2[index]);
                newLabels.Add(newLabel);
            }
            _newHashValues = newHashValues;
            _newPoints = new Dictionary<int, IReadOnlyList<Point>>();
            _newPoints.Add(_imageKey0, newPts0.AsReadOnly());
            _newPoints.Add(_imageKey1, newPts1.AsReadOnly());
            _newPoints.Add(_imageKey2, newPts2.AsReadOnly());
            _newLabels = newLabels;
        }

        [Conditional("DEBUG")]
        private void _DbgValidateNewPointCount()
        {
            if (_newPoints[_imageKey0].Count != _newPointCount ||
                _newPoints[_imageKey1].Count != _newPointCount ||
                _newPoints[_imageKey2].Count != _newPointCount ||
                _newLabels.Count != _newPointCount)
            {
                _DebugBreakOrThrow();
            }
        }

        [Conditional("DEBUG")]
        private void _DebugBreakOrThrow([CallerMemberName]string methodName = null)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                methodName = $"{nameof(T3Filter)}.{nameof(_DebugBreakOrThrow)}";
            }
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }
            else
            {
                throw new Exception(methodName);
            }
        }

        private void _CalculateUnmatchedPointCounts()
        {
            int rejectedPointCount = _oldPointCount - _newPointCount;
            var oldUnmatchedPointCounts = OldHashPoints.UnmatchedPointCounts;
            _unmatchedPointCounts = new Dictionary<int, int>(capacity: 3);
            _unmatchedPointCounts.Add(_imageKey0, oldUnmatchedPointCounts[_imageKey0] + rejectedPointCount);
            _unmatchedPointCounts.Add(_imageKey1, oldUnmatchedPointCounts[_imageKey1] + rejectedPointCount);
            _unmatchedPointCounts.Add(_imageKey2, oldUnmatchedPointCounts[_imageKey2] + rejectedPointCount);
        }

        private void _InitNewInstances()
        {
            NewHashPoints = new T3HashPoints(OldHashPoints.ImageKeys, _newHashValues, _newPoints, _unmatchedPointCounts.AsReadOnly());
            NewMovements = new T3Movements(NewHashPoints, _newMovementList, _newLabels.AsReadOnly(), new ReadOnlyDictionary<int, int>(_newLabelPointCounts));
        }
    }
}
