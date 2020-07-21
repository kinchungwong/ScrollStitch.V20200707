using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging
{
    using Data;

    public interface IArrayBitmap<T>
        : IScalarBitmapInfo
        where T : struct
    {
        T[] Data { get; }
        T this[Point p] { get;set; }
    }
}
