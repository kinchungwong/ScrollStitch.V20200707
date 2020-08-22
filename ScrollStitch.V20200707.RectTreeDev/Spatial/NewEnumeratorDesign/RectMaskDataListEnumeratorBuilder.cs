using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial.NewEnumeratorDesign
{
    using ScrollStitch.V20200707.Data;
    using ScrollStitch.V20200707.Functional;
    using ScrollStitch.V20200707.Spatial;
    using ScrollStitch.V20200707.Spatial.Internals;

    public static class RectMaskDataListEnumeratorBuilder<T>
    {
        public static RectMaskDataListEnumeratorBase<T> Create(
            IReadOnlyList<Rect> rects, 
            IReadOnlyList<RectMask128> masks,
            IReadOnlyList<T> data)
        {
            return new RectMaskDataListEnumeratorBase<T>(rects, masks, data);
        }

        public static RectMaskDataListEnumeratorBase<T> Create(
            IReadOnlyList<Rect> rects,
            IReadOnlyList<RectMask128> masks)
        {
            _CtorValidate_AndAssignCountAndNullList(rects, masks, out IReadOnlyList<T> data);
            return new RectMaskDataListEnumeratorBase<T>(rects, masks, data);
        }

        private static int _CtorValidate_AndAssignCountAndNullList(
            IReadOnlyList<Rect> rects,
            IReadOnlyList<RectMask128> masks,
            out IReadOnlyList<T> outData)
        {
            if (rects is null)
            {
                throw new ArgumentNullException(nameof(rects));
            }
            if (masks is null)
            {
                throw new ArgumentNullException(nameof(masks));
            }
            int rectCount = rects.Count;
            int maskCount = masks.Count;
            if (maskCount != rectCount)
            {
                throw new ArgumentException(paramName: nameof(masks), message: "Count mismatch.");
            }
            outData = new NullList<T>(capacity: rectCount, count: rectCount);
            return rectCount;
        }
    }
}
