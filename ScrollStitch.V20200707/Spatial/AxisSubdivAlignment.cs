using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    // ======
    // TODO WARNING
    // The following enum definition has not yet been sufficiently analyzed as to technical fitness-of-purpose.
    // ======

    [Flags]
    public enum AxisSubdivAlignment
    {
        /// <summary>
        /// A default value required for <see cref="FlagsAttribute"/> where none of the flag bits are set.
        /// </summary>
        None = 0,

        /// <summary>
        /// None of the technical requirements for any of the flag bits are met.
        /// 
        /// This value is also used if the <see cref="AxisSubdiv"/> is not bijective, meaning that 
        /// the range list contains gaps or overlaps.
        /// </summary>
        Unspecified = None,

        /// <summary>
        /// Technically left aligned.
        /// </summary>
        Left = 1,

        /// <summary>
        /// Technically right aligned.
        /// </summary>
        Right = 2,

        /// <summary>
        /// Technically aligned to left and right.
        /// </summary>
        Both = Left | Right,

        /// <summary>
        /// All except the first and the last ranges are aligned.
        /// </summary>
        Interior = 4,

        /// <summary>
        /// (This flag bit is only used in combination with other flag bits.)
        /// The technical requirement (for other flag bits) are met for the trivial reason that
        /// there are three or fewer ranges in the range list.
        /// </summary>
        Trivial = 8,

        /// <summary>
        /// 
        /// </summary>
        All = Both | Interior | Trivial
    }
}
