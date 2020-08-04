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

        #region automatic properties
        public int Left => Rect.Left;

        public int Center => Rect.Left + HalfSize.Width;

        public int Right => Rect.Right;

        public int Top => Rect.Top;

        public int Middle => Rect.Top + HalfSize.Height;

        public int Bottom => Rect.Bottom;
        #endregion

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Deconstruct(out int left, out int center, out int right, out int top, out int middle, out int bottom)
        {
            left = Left;
            center = Center;
            right = Right;
            top = Top;
            middle = Middle;
            bottom = Bottom;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public ItemFlag ClassifyItem(Rect itemRect) 
        {
            if (!itemRect.IsPositive)
            {
                _ThrowNonPositiveRect(itemRect, nameof(itemRect));
            }
            ItemFlag leftFlag = ClassifyItemLeft(itemRect.Left);
            ItemFlag rightFlag = ClassifyItemRight(itemRect.Right);
            ItemFlag topFlag = ClassifyItemTop(itemRect.Top);
            ItemFlag bottomFlag = ClassifyItemBottom(itemRect.Bottom);
            return (leftFlag & rightFlag) | (topFlag & bottomFlag);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ItemFlag ClassifyItemLeft(int itemLeft)
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
                    return ItemFlag.OutsideRight;
                }
                else
                {
                    return ItemFlag.RightMask;
                }
            }
            else
            {
                if (itemLeft >= boundLeft)
                {
                    return ItemFlag.InsideLeft | ItemFlag.RightMask;
                }
                else
                {
                    return ItemFlag.HorizontalMask;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ItemFlag ClassifyItemRight(int itemRight)
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
                    return ItemFlag.HorizontalMask;
                }
                else
                {
                    return ItemFlag.LeftMask | ItemFlag.InsideRight;
                }
            }
            else
            {
                if (itemRight > boundLeft)
                {
                    return ItemFlag.LeftMask;
                }
                else
                {
                    return ItemFlag.OutsideLeft;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ItemFlag ClassifyItemTop(int itemTop)
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
                    return ItemFlag.OutsideBottom;
                }
                else
                {
                    return ItemFlag.BottomMask;
                }
            }
            else
            {
                if (itemTop >= boundTop)
                {
                    return ItemFlag.InsideTop | ItemFlag.BottomMask;
                }
                else
                {
                    return ItemFlag.VerticalMask;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ItemFlag ClassifyItemBottom(int itemBottom)
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
                    return ItemFlag.VerticalMask;
                }
                else
                {
                    return ItemFlag.TopMask | ItemFlag.InsideBottom;
                }
            }
            else
            {
                if (itemBottom > boundTop)
                {
                    return ItemFlag.TopMask;
                }
                else
                {
                    return ItemFlag.OutsideTop;
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
