using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Arrays
{
    public interface IArray2Info
    {
        int Length { get; }

        int Length0 { get; }

        int Length1 { get; }

        int GetLength(int dim);
    }
}
