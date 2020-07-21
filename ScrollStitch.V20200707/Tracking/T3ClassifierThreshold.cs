using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    /// <summary>
    /// Specifies a set of threshold criteria that a three-image trajectory must satisfy
    /// in order to be valid.
    /// 
    /// <para>
    /// <seealso cref="T3Classifier"/> is the classifier that applies these threshold to movement 
    /// detections computed from three input images.
    /// </para>
    /// 
    /// <para>
    /// The classification result is described with values (bit flags) of the enum <seealso cref="T3ClassifierFlags"/>.
    /// </para>
    /// </summary>
    ///
    public class T3ClassifierThreshold
    {
        public int MinimumValidPointVote { get; set; } = 2;

        public double MinimumValidPointVoteFraction { get; set; } = 0.01;

        public int MinimumValidCellVote { get; set; } = 2;

        public double MinimumValidCellVoteFraction { get; set; } = 0.01;
    }
}
