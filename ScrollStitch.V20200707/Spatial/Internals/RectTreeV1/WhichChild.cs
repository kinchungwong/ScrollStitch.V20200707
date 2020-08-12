using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals.RectTreeV1
{
    using ScrollStitch.V20200707.Data;

    public enum WhichChild
    {
        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3,
        Straddle = 4
    }

    public static class WhichChildMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSplittable(this WhichChild whichChild)
        {
            switch (whichChild)
            {
                case WhichChild.TopLeft:
                case WhichChild.TopRight:
                case WhichChild.BottomLeft:
                case WhichChild.BottomRight:
                    return true;
                default:
                    return false;
            }
        }

        public static string ToShortString(this WhichChild whichChild)
        {
            switch (whichChild)
            {
                case WhichChild.TopLeft:
                    return "TL";
                case WhichChild.TopRight:
                    return "TR";
                case WhichChild.BottomLeft:
                    return "BL";
                case WhichChild.BottomRight:
                    return "BR";
                default:
                    return "XX";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static WhichChild FlagToChild(this ItemFlag flag)
        {
            switch (flag)
            {
                case (ItemFlag.InsideLeft | ItemFlag.InsideTop):
                    return WhichChild.TopLeft;
                case (ItemFlag.InsideRight | ItemFlag.InsideTop):
                    return WhichChild.TopRight;
                case (ItemFlag.InsideLeft | ItemFlag.InsideBottom):
                    return WhichChild.BottomLeft;
                case (ItemFlag.InsideRight | ItemFlag.InsideBottom):
                    return WhichChild.BottomRight;
                default:
                    return WhichChild.Straddle;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ItemFlag ChildToFlag(this WhichChild whichChild)
        {
            switch (whichChild)
            {
                case WhichChild.TopLeft:
                    return (ItemFlag.InsideLeft | ItemFlag.InsideTop);
                case WhichChild.TopRight:
                    return (ItemFlag.InsideRight | ItemFlag.InsideTop);
                case WhichChild.BottomLeft:
                    return (ItemFlag.InsideLeft | ItemFlag.InsideBottom);
                case WhichChild.BottomRight:
                    return (ItemFlag.InsideRight | ItemFlag.InsideBottom);
                case WhichChild.Straddle:
                    return ItemFlag.InsideMask;
                default:
                    return ItemFlag.None;
            }
        }
    }
}
