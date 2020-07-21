using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.IO
{
    public class Bgr888ToGray8
    {
        public LockedByteBitmap Target { get; set; }

        public Bgr888ToGray8(LockedByteBitmap target)
        {
            Target = target;
            if (Target.ArrayElementsPerRow != Target.Width * 3)
            {
                throw new InvalidOperationException();
            }
        }

        public void CopyRow(int row, byte[] dest, int destStart)
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
                int intGray = (red + green + blue) / 3;
                byte gray = unchecked((byte)intGray);
                dest[destStart + k] = gray;
            }
        }

        public void WriteRow(int row, byte[] source, int sourceStart)
        {
            int imageWidth = Target.Width;
            int sourceElementsPerRow = Target.ArrayElementsPerRow;
            int destCount = imageWidth;
            byte[] buffer = new byte[sourceElementsPerRow];
            for (int k = destCount - 1; k >= 0; --k)
            {
                byte gray = source[sourceStart + k];
                buffer[k * 3 + 2] = gray;
                buffer[k * 3 + 1] = gray;
                buffer[k * 3] = gray;
            }
            Target.WriteRow(row, buffer, 0);
        }
    }
}
