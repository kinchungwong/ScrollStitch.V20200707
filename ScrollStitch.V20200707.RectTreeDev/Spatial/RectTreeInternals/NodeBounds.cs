using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.RectTreeInternals
{
    using ScrollStitch.V20200707.Data;

    public struct NodeBounds
    {
        /// <summary>
        /// It is impossible to create an instance where the rect width or height 
        /// is zero or one.
        /// </summary>
        public static int AllowedMinRectLength = 2;

        public Rect Rect { get; }
        
        public Size HalfSize { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NodeBounds(Rect rect)
            : this(rect, new Size(rect.Width / 2, rect.Height / 2))
        {
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NodeBounds(Rect rect, Size halfSize)
        {
            _CtorValidate(rect, halfSize);
            Rect = rect;
            HalfSize = halfSize;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ItemFlags ClassifyItem(Rect itemRect) 
        {
            if (!itemRect.IsPositive)
            {
                _ThrowNonPositiveRect(itemRect, nameof(itemRect));
            }
            ItemFlags leftFlag = ClassifyItemLeft(itemRect.Left);
            ItemFlags rightFlag = ClassifyItemRight(itemRect.Right);
            ItemFlags topFlag = ClassifyItemTop(itemRect.Top);
            ItemFlags bottomFlag = ClassifyItemBottom(itemRect.Bottom);
            return (leftFlag & rightFlag) | (topFlag & bottomFlag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ItemFlags ClassifyItemLeft(int itemLeft)
        {
            //const ItemFlags hhhh = ItemFlags.HorizontalMask;
            //const ItemFlags _hhh = ItemFlags.InsideLeft | ItemFlags.RightMask;
            //const ItemFlags __hh = ItemFlags.RightMask;
            //const ItemFlags ___h = ItemFlags.OutsideRight;
            int boundLeft = Rect.Left;
            int boundCenter = Rect.Left + HalfSize.Width;
            int boundRight = Rect.Right;
            if (itemLeft >= boundCenter)
            {
                if (itemLeft >= boundRight)
                {
                    return ItemFlags.OutsideRight;
                }
                else
                {
                    return ItemFlags.RightMask;
                }
            }
            else
            {
                if (itemLeft >= boundLeft)
                {
                    return ItemFlags.InsideLeft | ItemFlags.RightMask;
                }
                else
                {
                    return ItemFlags.HorizontalMask;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ItemFlags ClassifyItemRight(int itemRight)
        {
            //const ItemFlags h___ = ItemFlags.OutsideLeft;
            //const ItemFlags hh__ = ItemFlags.LeftMask;
            //const ItemFlags hhh_ = ItemFlags.LeftMask | ItemFlags.InsideRight;
            //const ItemFlags hhhh = ItemFlags.HorizontalMask;
            int boundLeft = Rect.Left;
            int boundCenter = Rect.Left + HalfSize.Width;
            int boundRight = Rect.Right;
            if (itemRight > boundCenter)
            {
                if (itemRight > boundRight)
                {
                    return ItemFlags.HorizontalMask;
                }
                else
                {
                    return ItemFlags.LeftMask | ItemFlags.InsideRight;
                }
            }
            else
            {
                if (itemRight > boundLeft)
                {
                    return ItemFlags.LeftMask;
                }
                else
                {
                    return ItemFlags.OutsideLeft;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ItemFlags ClassifyItemTop(int itemTop)
        {
            //const ItemFlags vvvv = ItemFlags.VerticalMask;
            //const ItemFlags _vvv = ItemFlags.InsideTop | ItemFlags.BottomMask;
            //const ItemFlags __vv = ItemFlags.BottomMask;
            //const ItemFlags ___v = ItemFlags.OutsideBottom;
            int boundTop = Rect.Top;
            int boundMiddle = Rect.Top + HalfSize.Height;
            int boundBottom = Rect.Bottom;
            if (itemTop >= boundMiddle)
            {
                if (itemTop >= boundBottom)
                {
                    return ItemFlags.OutsideBottom;
                }
                else
                {
                    return ItemFlags.BottomMask;
                }
            }
            else
            {
                if (itemTop >= boundTop)
                {
                    return ItemFlags.InsideTop | ItemFlags.BottomMask;
                }
                else
                {
                    return ItemFlags.VerticalMask;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ItemFlags ClassifyItemBottom(int itemBottom)
        {
            //const ItemFlags v___ = ItemFlags.OutsideTop;
            //const ItemFlags vv__ = ItemFlags.TopMask;
            //const ItemFlags vvv_ = ItemFlags.TopMask | ItemFlags.InsideBottom;
            //const ItemFlags vvvv = ItemFlags.VerticalMask;
            int boundTop = Rect.Top;
            int boundMiddle = Rect.Top + HalfSize.Height;
            int boundBottom = Rect.Bottom;
            if (itemBottom > boundMiddle)
            {
                if (itemBottom > boundBottom)
                {
                    return ItemFlags.VerticalMask;
                }
                else
                {
                    return ItemFlags.TopMask | ItemFlags.InsideBottom;
                }
            }
            else
            {
                if (itemBottom > boundTop)
                {
                    return ItemFlags.TopMask;
                }
                else
                {
                    return ItemFlags.OutsideTop;
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void _CtorValidate(Rect rect, Size halfSize)
        {
            if (rect.Width <= 0 ||
                rect.Height <= 0)
            {
                _ThrowNonPositiveRect(rect, nameof(rect));
            }
            if (halfSize.Width <= 0 ||
                halfSize.Height <= 0)
            {
                _ThrowNonPositiveHalfSize(halfSize, nameof(halfSize));
            }
            if (halfSize.Width >= rect.Width ||
                halfSize.Height >= rect.Height)
            {
                _ThrowInvalidHalfSize(rect, halfSize);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void _ThrowNonPositiveRect(Rect rect, string paramName)
        {
            throw new ArgumentException(
                paramName: paramName,
                message: "This method requires positive rect width and height. Actual: " + rect.ToString());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void _ThrowNonPositiveHalfSize(Size halfSize, string paramName)
        {
            throw new ArgumentException(
                paramName: paramName,
                message: "This method requires positive halfSize width and height. Actual: " + halfSize.ToString());
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void _ThrowInvalidHalfSize(Rect rect, Size halfSize)
        {
            throw new ArgumentException(
                message: "This method requires half size to be strictly less than rect size. " + 
                $"Actual rect: {rect}, " + 
                $"Actual halfSize: {halfSize}");
        }
    }
}
