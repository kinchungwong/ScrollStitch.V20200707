using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    public interface IGridCollection
    {
        Grid Grid { get; }
        int GridWidth { get; }
        int GridHeight { get; }
    }

    public interface IGridCollection<T>
        : IGridCollection
    {
        T this[CellIndex ci] { get; set; }
    }

    public interface IReadOnlyGridCollection<T>
        : IGridCollection
    {
        T this[CellIndex ci] { get; }
    }
}
