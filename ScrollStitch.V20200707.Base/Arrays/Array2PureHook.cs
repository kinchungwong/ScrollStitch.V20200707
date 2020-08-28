using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Arrays
{
    /// <summary>
    /// An implementation of the <see cref="IArray2{T}"/> interface that relies solely on the 
    /// item getter and setter functions. This implementation does not have direct access to an 
    /// underlying array.
    /// </summary>
    /// 
    /// <typeparam name="T">The item type exposed on the interface on this instance.</typeparam>
    /// 
    public class Array2PureHook<T>
        : IArray2<T>
        , IReadOnlyArray2<T>
    {
        /// <summary>
        /// A function that is called when the array's getter is called.
        /// </summary>
        public delegate T ItemGetDelegate(int idx0, int idx1);

        /// <summary>
        /// A function that is called when the array's setter is called.
        /// </summary>
        public delegate void ItemUpdateDelegate(int idx0, int idx1, T value);

        public long LongLength => (long)Length0 * Length1;

        public int Length => checked((int)LongLength);

        public int Length0 { get; }

        public int Length1 { get; }

        public int GetLength(int dim)
        {
            switch (dim)
            {
                case 0: 
                    return Length0;
                case 1:
                    return Length1;
                default:
                    throw new IndexOutOfRangeException(message: nameof(dim));
            }
        }

        /// <summary>
        /// <inheritdoc cref="ItemGetDelegate"/>
        /// 
        /// <para>
        /// Refer to <seealso cref="ItemGetDelegate"/> for details.
        /// </para>
        /// </summary>
        public ItemGetDelegate GetFunc { get; }

        /// <summary>
        /// <inheritdoc cref="ItemUpdateDelegate"/>
        /// 
        /// <para>
        /// Refer to <seealso cref="ItemUpdateDelegate"/> for details.
        /// </para>
        /// </summary>
        public ItemUpdateDelegate UpdateFunc { get; }

        /// <summary>
        /// Item getter and setter. These will trigger the hook functions. 
        /// 
        /// <para>
        /// Calling the getter triggers <see cref="GetFunc"/>.
        /// <br/>
        /// Calling the setter triggers <see cref="UpdateFunc"/>.
        /// </para>
        /// </summary>
        /// 
        /// <param name="idx0"></param>
        /// <param name="idx1"></param>
        /// <returns></returns>
        /// 
        public T this[int idx0, int idx1]
        {
            get
            {
                return GetFunc(idx0, idx1);
            }
            set
            {
                UpdateFunc(idx0, idx1, value);
            }
        }

        public Array2PureHook(int length0, int length1, Func<int, int, T> getFunc, Action<int, int, T> updateFunc)
        {
            _ValidateLengths(length0, length1);
            _ValidateFuncs(getFunc, updateFunc);
            Length0 = length0;
            Length1 = length1;
            GetFunc = new ItemGetDelegate(getFunc);
            UpdateFunc = new ItemUpdateDelegate(updateFunc);
        }

        public Array2PureHook(int length0, int length1, ItemGetDelegate getFunc, ItemUpdateDelegate updateFunc)
        {
            _ValidateLengths(length0, length1);
            _ValidateFuncs(getFunc, updateFunc);
            Length0 = length0;
            Length1 = length1;
            GetFunc = getFunc;
            UpdateFunc = updateFunc;
        }

        private static void _ValidateLengths(int length0, int length1)
        {
            if (length0 < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length0));
            }
            if (length1 < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length1));
            }
            long longLength = (long)length0 * length1;
            int length = checked((int)longLength);
            if (length != longLength)
            {
                throw new OverflowException(nameof(length));
            }
        }

        private static void _ValidateFuncs<TFunc1, TFunc2>(TFunc1 getFunc, TFunc2 updateFunc)
            where TFunc1: Delegate
            where TFunc2: Delegate
        {
            if (getFunc is null)
            {
                throw new ArgumentNullException(nameof(getFunc));
            }
            if (updateFunc is null)
            {
                throw new ArgumentNullException(nameof(updateFunc));
            }
        }
    }
}
