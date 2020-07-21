using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Plotting
{
    public interface IDrawCmd
    {
        void Draw(IntBitmap dest);
    }
}
