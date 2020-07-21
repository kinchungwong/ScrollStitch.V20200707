using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Data
{
    using HashCode;

    /// <summary>
    /// 
    /// 
    /// Refer to <code>Readme_Hash2D.Data_namespace.md</code>
    /// </summary>
    public struct Size
        : IEquatable<Size>
    {
        public int Width { get; }
        public int Height { get; }

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Size other:
                    return Equals(other);
                default:
                    return false;
            }
        }

        public bool Equals(Size other)
        {
            return Width == other.Width &&
                Height == other.Height;
        }

        public static bool operator ==(Size s1, Size s2)
        {
            return s1.Equals(s2);
        }

        public static bool operator !=(Size s1, Size s2)
        {
            return !s1.Equals(s2);
        }

        public static implicit operator System.Drawing.Size(Size s)
        {
            return new System.Drawing.Size(s.Width, s.Height);
        }

        public static implicit operator Size(System.Drawing.Size s)
        {
            return new Size(s.Width, s.Height);
        }

        public override int GetHashCode()
        {
            return HashCodeBuilder.ForType<Size>().Ingest(Width, Height).GetHashCode();
        }

        public override string ToString()
        {
            return $"(W={Width}, H={Height})";
        }
    }
}
