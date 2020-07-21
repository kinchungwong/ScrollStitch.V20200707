using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Arrays
{
    public class Array2Hook<T>
        : IArray2<T>
        , IReadOnlyArray2<T>
    {
        /// <summary>
        /// A function that is called when the array's getter is called.
        /// 
        /// </summary>
        /// <param name="storedValue">
        /// The item that is retrieved from the underlying array, <see cref="Target"/>.
        /// </param>
        /// <returns>
        /// The item that will actually be returned to the caller.
        /// </returns>
        public delegate T ItemGetDelegate(T storedValue);

        /// <summary>
        /// A function that is called when the array's setter is called.
        /// </summary>
        /// <param name="oldValue">
        /// The original value from the underlying array, <see cref="Target"/>.
        /// </param>
        /// <param name="newValue">
        /// The new value specified by the caller when the setter is called.
        /// </param>
        /// <returns>
        /// The final value that will be stored into the underlying array.
        /// </returns>
        public delegate T ItemUpdateDelegate(T oldValue, T newValue);

        public IArray2<T> Target { get; }

        public int Length => Target.Length;

        public int Length0 => Target.Length0;

        public int Length1 => Target.Length1;

        public int GetLength(int dim) => Target.GetLength(dim);

        /// <summary>
        /// <inheritdoc cref="ItemGetDelegate"/>
        /// Refer to <seealso cref="ItemGetDelegate"/> for details.
        /// </summary>
        public Func<T, T> GetFunc { get; }

        /// <summary>
        /// <inheritdoc cref="ItemUpdateDelegate"/>
        /// Refer to <seealso cref="ItemUpdateDelegate"/> for details.
        /// </summary>
        public Func<T, T, T> UpdateFunc { get; }

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
                return GetFunc(Target[idx0, idx1]);
            }
            set
            {
                T oldValue = Target[idx0, idx1];
                Target[idx0, idx1] = UpdateFunc(oldValue, value);
            }
        }

        public Array2Hook(IArray2<T> target, Func<T, T> getFunc, Func<T, T, T> updateFunc)
        {
            Target = target;
            GetFunc = getFunc;
            UpdateFunc = updateFunc;
        }

        public Array2Hook(IArray2<T> target, ItemGetDelegate getFunc, ItemUpdateDelegate updateFunc)
        {
            Target = target;
            GetFunc = new Func<T, T>(getFunc);
            UpdateFunc = new Func<T, T, T>(updateFunc);
        }
    }
}
