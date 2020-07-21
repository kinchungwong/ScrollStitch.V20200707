using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.RowAccess
{
    public interface IBitmapRowDirect<T>
        : IBitmapRowAccess<T>
        where T : struct
    {
#if false
        #region inherited
        int Width { get; }
        int Height { get; }
        Size Size { get; }
        Type DataType { get; }
        Type ArrayType { get; }
        bool CanRead { get; }
        bool CanWrite { get; }
        void CopyRow(int row, T[] dest, int destStart);
        void WriteRow(int row, T[] source, int sourceStart);
        #endregion
#endif

        ArraySegment<T> GetRowDirect(int row);
    }
}
