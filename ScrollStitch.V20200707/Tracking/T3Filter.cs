using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
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

        public T3Filter(T3HashPoints oldHashPoints, T3Movements oldMovements, T3Classifier classifier)
        {
            OldHashPoints = oldHashPoints;
            OldMovements = oldMovements;
            Classifier = classifier;
        }

        public void Process()
        { 
        }
    }
}
