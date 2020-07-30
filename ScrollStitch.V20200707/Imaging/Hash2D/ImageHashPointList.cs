using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Imaging.Hash2D
{
    using Data;
    using V20200707.Collections;

    public class ImageHashPointList 
        : IReadOnlyList<HashPoint>
    {
        /// <summary>
        /// The image ID from which the list of hash points is generated.
        /// 
        /// This property is mutable because the class that computes the hash points 
        /// may not know the image ID; it has to be assigned further up the caller tree.
        /// </summary>
        public int ImageIndex { get; set; }

        /// <summary>
        /// The list of hash points. Each hash point is an image coordinate and a 2D hash code 
        /// generated from the pixels around that coordinate.
        /// </summary>
        public IReadOnlyList<HashPoint> HashPoints { get; }

        public HashPoint this[int index] => HashPoints[index];

        public int Count => HashPoints.Count;

        public ImageHashPointList(IReadOnlyList<HashPoint> hashPoints)
        {
            HashPoints = hashPoints;
        }

        public ImageHashPointList(IReadOnlyList<HashPoint> hashPoints, int imageIndex)
        {
            HashPoints = hashPoints;
            ImageIndex = imageIndex;
        }

        public IEnumerator<HashPoint> GetEnumerator()
        {
            return HashPoints.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)HashPoints).GetEnumerator();
        }

        public IReadOnlyCollection<ImageHashPoint> AsImageHashPoints()
        {
            IEnumerable<ImageHashPoint> EnumeratorFunc()
            {
                for (int k = 0; k < Count; ++k)
                {
                    var hp = HashPoints[k];
                    yield return new ImageHashPoint(ImageIndex, hp.HashValue, hp.Point);
                }
            }
            return new ReadOnlyEnumerableCollection<ImageHashPoint>(Count, EnumeratorFunc());
        }

        public List<ImageHashPoint> ToImageHashPointList()
        {
            var imageHashPoints = AsImageHashPoints();
            List<ImageHashPoint> list = new List<ImageHashPoint>(capacity: imageHashPoints.Count);
            list.AddRange(imageHashPoints);
            return list;
        }
    }
}
