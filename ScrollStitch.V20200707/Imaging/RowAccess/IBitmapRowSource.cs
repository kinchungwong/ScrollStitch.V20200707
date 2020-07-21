using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.RowAccess
{
    public interface IBitmapRowSource<T>
        : IScalarBitmapInfo
        where T : struct
    {
#if false
        #region inherited
        int Width { get; }
        int Height { get; }
        Size Size { get; }
        Type DataType { get; }
        Type ArrayType { get; }
        #endregion
#endif
        bool CanRead { get; }

        void CopyRow(int row, T[] dest, int destStart);
    }
}
