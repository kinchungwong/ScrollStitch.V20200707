using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    public class ImagePairMovementScore
    {
        /// <summary>
        /// A confidence score for the hypothesis that hardly anything changed or moved 
        /// between the two images.
        /// 
        /// This is a confidence score normalized to a scale of 0.0 to 1.0 inclusive.
        /// </summary>
        public double StillScore { get; set; }

        /// <summary>
        /// A confidence score for the hypothesis that a relevant (not insignificant) part of 
        /// the screen content has moved between the two images.
        /// 
        /// This is a confidence score normalized to a scale of 0.0 to 1.0 inclusive.
        /// </summary>
        public double MovedScore { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public double MovedChurn { get; set; }

        /// <summary>
        /// A confidence score for the hypothesis that a screen switch has occured, that
        /// a signficant portion of the screen content have changed.
        /// 
        /// This is a confidence score normalized to a scale of 0.0 to 1.0 inclusive.
        /// </summary>
        public double BreakScore { get; set; }

        /// <summary>
        /// The fraction of screen content that has not moved between the two images.
        /// </summary>
        public double StationaryFrac { get; set; }
    }
}
