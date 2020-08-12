using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using Data;

    public class PointTree<T>
    {
        #region private
        private IRectCollectionEx<(Point, T)> _rectTree;
        #endregion

        public PointTree()
        {
            _rectTree = new RectList<(Point, T)>(((Point, T) pt) => new Rect(pt.Item1.X, pt.Item1.Y, 1, 1));
        }

        public void Add(Point p, T t)
        {
            _rectTree.Add((p, t));
        }

        public void ForEach(Rect searchRect, Action<(Point, T)> action)
        {
            _rectTree.ForEach(searchRect, action);
        }
    }
}
