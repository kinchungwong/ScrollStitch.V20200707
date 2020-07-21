using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Tracking
{
    [Flags]
    public enum T3ClassifierFlags
    {
        None = 0,
        Accepted = 1,
        Stationary = 2,
        RejectedTooFewPoints = 4,
        RejectedTooFewCells = 8
    }
}
