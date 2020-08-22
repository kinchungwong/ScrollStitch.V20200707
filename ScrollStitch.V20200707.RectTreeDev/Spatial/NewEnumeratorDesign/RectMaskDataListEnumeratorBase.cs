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

    /// <summary>
    /// 
    /// 
    /// <para>
    /// Design note.
    /// <br/>
    /// This class is not sealed, by design.
    /// </para>
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// </typeparam>
    /// 
    public class RectMaskDataListEnumeratorBase<T>
        : IEnumerator<(int, Rect, RectMask128, T)>
        , IEnumerator<(Rect, T)>
    {
        public IReadOnlyList<Rect> Rects { get; }
        
        public IReadOnlyList<RectMask128> Masks { get; }
        
        public IReadOnlyList<T> Data { get; }

        public int Count { get; }

        public int CurrentIndex { get; private set; }

        public Rect CurrentRect => Rects[CurrentIndex];

        public RectMask128 CurrentMask => Masks[CurrentIndex];

        public T CurrentData => Data[CurrentIndex];

        public (int, Rect, RectMask128, T) Current => 
            (CurrentIndex, CurrentRect, CurrentMask, CurrentData);

        (Rect, T) IEnumerator<(Rect, T)>.Current =>
            (CurrentRect, CurrentData);

        object IEnumerator.Current => Current;

        public RectMaskDataListEnumeratorBase(IReadOnlyList<Rect> rects, IReadOnlyList<RectMask128> masks, 
            IReadOnlyList<T> data)
        {
            Count = _CtorValidate_AndGetCount(rects, masks, data);
            Rects = Rects;
            Masks = Masks;
            Data = Data;
            CurrentIndex = -1;
        }

        public bool MoveNext()
        {
            _ValidateCurrentIndex();
            if (Count == 0)
            {
                CurrentIndex = 0;
                return false;
            }
            while (HasNext())
            {
                _CheckedIncrementCurrentIndex();
                return true;
            }
            return false;
        }

        public bool MoveNext(Func<int, Rect, RectMask128, T, bool> filterPred)
        {
            _ValidateCurrentIndex();
            if (Count == 0)
            {
                CurrentIndex = 0;
                return false;
            }
            while (HasNext())
            {
                _CheckedIncrementCurrentIndex();
                if (filterPred.Invoke(CurrentIndex, CurrentRect, CurrentMask, CurrentData))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterPred"></param>
        /// <returns></returns>
        public bool MoveNext(IFunc<int, Rect, RectMask128, T, bool> filterPred)
        {
            _ValidateCurrentIndex();
            if (Count == 0)
            {
                CurrentIndex = 0;
                return false;
            }
            while (HasNext())
            {
                _CheckedIncrementCurrentIndex();
                if (filterPred.Invoke(CurrentIndex, CurrentRect, CurrentMask, CurrentData))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterPred"></param>
        /// <returns></returns>
        public bool MoveNext(RectMaskRelationPredicate relationPred)
        {
            _ValidateCurrentIndex();
            if (Count == 0)
            {
                CurrentIndex = 0;
                return false;
            }
            while (HasNext())
            {
                _CheckedIncrementCurrentIndex();
                if (relationPred.Invoke(CurrentRect, CurrentMask))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filterPred"></param>
        /// <returns></returns>
        public bool MoveNext<TFilterPred>(TFilterPred filterPred)
            where TFilterPred : struct, IFuncInline<TFilterPred, int, Rect, RectMask128, T, bool>
        {
            _ValidateCurrentIndex();
            if (Count == 0)
            {
                CurrentIndex = 0;
                return false;
            }
            while (HasNext())
            {
                _CheckedIncrementCurrentIndex();
                if (filterPred.Invoke(CurrentIndex, CurrentRect, CurrentMask, CurrentData))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns whether, by comparing <see cref="CurrentIndex"/> and <see cref="Count"/> alone,
        /// there is a next element in the enumerator, without considering the filter predicate.
        /// </summary>
        /// <returns>
        /// The result of the expression <c>(CurrentIndex + 1 &lt; Count)</c>.
        /// </returns>
        public bool HasNext()
        {
            checked
            {
                return CurrentIndex + 1 < Count;
            }
        }

        public void Dispose()
        {
        }

        public void Reset()
        {
            CurrentIndex = -1;
        }

        private void _CheckedIncrementCurrentIndex()
        {
            checked
            {
                ++CurrentIndex;
            }
        }

        private void _ValidateCurrentIndex()
        {
            if (CurrentIndex < -1 ||
                CurrentIndex > Count)
            {
                throw new Exception("Unexpected");
            }
        }

        private static int _CtorValidate_AndGetCount(IReadOnlyList<Rect> rects, 
            IReadOnlyList<RectMask128> masks, IReadOnlyList<T> data)
        {
            if (rects is null)
            {
                throw new ArgumentNullException(nameof(rects));
            }
            if (masks is null)
            {
                throw new ArgumentNullException(nameof(masks));
            }
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }
            int rectCount = rects.Count;
            int maskCount = masks.Count;
            int dataCount = data.Count;
            if (maskCount != rectCount)
            {
                throw new ArgumentException(paramName: nameof(masks), message: "Count mismatch.");
            }
            if (dataCount != rectCount)
            {
                throw new ArgumentException(paramName: nameof(data), message: "Count mismatch.");
            }
            return rectCount;
        }
    }
}
