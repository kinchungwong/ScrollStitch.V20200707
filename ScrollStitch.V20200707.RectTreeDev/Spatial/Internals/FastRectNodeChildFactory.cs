using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.Internals
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;

    /// <summary>
    /// Fast Rect Node is Evil! 
    /// Abolish Child Factory!
    /// </summary>
    public class FastRectNodeChildFactory
    {
        public IReadOnlyList<FastRectNodeChildSpec> ChildSpecs { get; }

        public class Builder
        {
            public SortedSet<FastRectNodeChildSpec> ChildSpecs { get; } = new SortedSet<FastRectNodeChildSpec>();

            public Builder Add(FastRectNodeChildSpec childSpec)
            {
                ChildSpecs.Add(childSpec);
                return this;
            }

            public Builder Add(
                int horzParentCellCount, int vertParentCellCount,
                int horzChildCellCount, int vertChildCellCount,
                int horzCellShift, int vertCellShift)
            {
                var childSpec = new FastRectNodeChildSpec(
                    horzParentCellCount: horzParentCellCount,
                    vertParentCellCount: vertParentCellCount,
                    horzChildCellCount: horzChildCellCount,
                    vertChildCellCount: vertChildCellCount,
                    horzCellShift: horzCellShift,
                    vertCellShift: vertCellShift);
                ChildSpecs.Add(childSpec);
                return this;
            }

            public FastRectNodeChildFactory Create()
            {
                return new FastRectNodeChildFactory(ChildSpecs.ToList());
            }
        }

        public FastRectNodeChildFactory(IReadOnlyList<FastRectNodeChildSpec> childSpecs)
        {
            // ====== Order of child rectangle insertion ======
            // During item processing in _ProcessNewData(), each item is sent to the first 
            // child node on the list whose bounding rectangle encompasses the item's rectangle.
            //
            // Thus, to maximize specificity (query efficiency), the child nodes with the most 
            // specific (tiniest) bounding rectangle should be checked upfront so that they are 
            // preferentially selected.
            //
            // The child node's specificity is proportional to its child-to-parent area ratio.
            // Thus, ChildSpec's CompareTo() implementation is based on that ratio.
            // ======
            var list = new UniqueList<FastRectNodeChildSpec>(childSpecs).ToList();
            list.Sort();
            ChildSpecs = list.AsReadOnly();
        }

        /// <summary>
        /// Given the parent node's bounding rectangle, enumerate all child rectangles based on the 
        /// spec parameters.
        /// 
        /// <para>
        /// Remark. 
        /// <br/>
        /// This function filters out child bounding rectangles which are exact duplicates. This filtering
        /// is based on comparison of their coordinates.
        /// <br/>
        /// The exact number of child nodes generated will be dependent on the specified parent bounding
        /// rectangle. When the parent bounding rectangle is too small, some child nodes may not be 
        /// generated. 
        /// <br/>
        /// Moreover, integer rounding issues may give rise to child rectangles whose bounds only differ 
        /// by a single pixel, though this issue occurs very rarely.
        /// </para>
        /// </summary>
        /// 
        /// <param name="boundingRect">
        /// Parent node's bounding rectangle.
        /// </param>
        /// 
        /// <returns>
        /// An enumeration of distinct child node bounding rectangles generated according to the spec,
        /// sorted by their nominal area ratio, with the ones having smallest area ratios listed earlier.
        /// </returns>
        /// 
        public IEnumerable<Rect> Enumerate(Rect boundingRect)
        {
            var hasGeneratedRect = new HashSet<Rect>();
            foreach (var childSpec in ChildSpecs)
            {
                foreach (var childRect in childSpec.Enumerate(boundingRect))
                {
                    if (hasGeneratedRect.Contains(childRect))
                    {
                        continue;
                    }
                    hasGeneratedRect.Add(childRect);
                    yield return childRect;
                }
            }
        }
    }
}
