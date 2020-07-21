using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.IO
{
    public class Bgrx8888ToGray8
    {
        public LockedByteBitmap Target { get; set; }

        public Bgrx8888ToGray8(LockedByteBitmap target)
        {
            Target = target;
            if (Target.ArrayElementsPerRow != Target.Width * 4)
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
                byte red = buffer[k * 4 + 2];
                byte green = buffer[k * 4 + 1];
                byte blue = buffer[k * 4];
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
                buffer[k * 4 + 2] = gray;
                buffer[k * 4 + 1] = gray;
                buffer[k * 4] = gray;
            }
            Target.WriteRow(row, buffer, 0);
        }
    }
}
