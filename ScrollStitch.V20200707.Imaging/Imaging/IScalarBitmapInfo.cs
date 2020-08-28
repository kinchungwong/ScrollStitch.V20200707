using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging
{
    using Data;

    public interface IScalarBitmapInfo
    {
        int Width { get; }
        int Height { get; }
        Size Size { get; }
        Type DataType { get; }
        Type ArrayType { get; }
    }
}
