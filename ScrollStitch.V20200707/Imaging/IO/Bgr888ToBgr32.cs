using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.IO
{
    public class Bgr888ToBgr32
    {
        public LockedByteBitmap Target { get; set; }

        public Bgr888ToBgr32(LockedByteBitmap target)
        {
            Target = target;
            if (Target.ArrayElementsPerRow != Target.Width * 3)
            {
                throw new InvalidOperationException();
            }
        }

        public void CopyRow(int row, int[] dest, int destStart)
        {
            int imageWidth = Target.Width;
            int sourceElementsPerRow = Target.ArrayElementsPerRow;
            int destCount = imageWidth;
            byte[] buffer = new byte[sourceElementsPerRow];
            Target.CopyRow(row, buffer, 0);
            for (int k = destCount - 1; k >= 0; --k)
            {
                byte red = buffer[k * 3 + 2];
                byte green = buffer[k * 3 + 1];
                byte blue = buffer[k * 3];
                uint bgr32 = blue | ((uint)green << 8) | ((uint)red << 16);
                dest[destStart + k] = unchecked((int)bgr32);
            }
        }

        public void WriteRow(int row, int[] source, int sourceStart)
        {
            int imageWidth = Target.Width;
            int sourceElementsPerRow = Target.ArrayElementsPerRow;
            int destCount = imageWidth;
            byte[] buffer = new byte[sourceElementsPerRow];
            for (int k = destCount - 1; k >= 0; --k)
            {
                int intBgr32 = source[sourceStart + k];
                uint bgr32 = unchecked((uint)intBgr32);
                byte blue = unchecked((byte)(bgr32 & 0xFFu));
                byte green = unchecked((byte)((bgr32 >> 8) & 0xFFu));
                byte red = unchecked((byte)((bgr32 >> 16) & 0xFFu));
                buffer[k * 3 + 2] = red;
                buffer[k * 3 + 1] = green;
                buffer[k * 3] = blue;

            }
            Target.WriteRow(row, buffer, 0);
        }
    }
}
