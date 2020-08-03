using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.RectTreeInternals
{
    [Flags]
    public enum ItemFlags
    {
        None = 0,
        OutsideLeft = 1,
        InsideLeft = 2,
        InsideRight = 4,
        OutsideRight = 8,
        OutsideTop = 16,
        InsideTop = 32,
        InsideBottom = 64,
        OutsideBottom = 128,
        LeftMask = OutsideLeft | InsideLeft,
        RightMask = OutsideRight | InsideRight,
        TopMask = OutsideTop | InsideTop,
        BottomMask = OutsideBottom | InsideBottom,
        HorizontalMask = LeftMask | RightMask,
        VerticalMask = TopMask | BottomMask,
        InsideMask = InsideLeft | InsideRight | InsideTop | InsideBottom,
        OutsideMask = OutsideLeft | OutsideRight | OutsideTop | OutsideBottom,
        All = InsideMask | OutsideMask
    }
}
