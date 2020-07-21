using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Arrays
{
    /// <summary>
    /// A wrapper that provides an <see cref="IArray2{T}"/> interface that internally wraps a target array
    /// of a different underlying type, and provides hooks for the item getter and setter functions.
    /// 
    /// <para>
    /// The item getter and setter hook functions are responsible for the conversion between the 
    /// interface item type <see cref="T"/> and the underlying item type <see cref="UT"/>.
    /// </para>
    /// 
    /// <example>
    /// <para>
    /// The item getter and setter functions are used in this class in the following way. <br/>
    /// (Type annotations are added superfluously in order to enhance clarity.)
    /// </para>
    /// <para><code>
    /// L001    public T this[int idx0, int idx1] { <br/>
    /// L002        get => GetFunc(Target[idx0, idx1] as UT) as T; <br/>
    /// L003        set => Target[idx0, idx1] = UpdateFunc(Target[idx0, idx1] as UT, value as T) as UT; <br/>
    /// L004    } <br/>
    /// </code></para>
    /// </example>
    /// </summary>
    /// 
    /// <typeparam name="T">The item type exposed on the interface on this instance.</typeparam>
    /// <typeparam name="UT">The underlying item type stored in the target array.</typeparam>
    /// 
    public class Array2HeteroHook<T, UT>
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
        public delegate T ItemGetDelegate(UT storedValue);

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
        public delegate UT ItemUpdateDelegate(UT oldValue, T newValue);

        public IArray2<UT> Target { get; }

        public int Length => Target.Length;

        public int Length0 => Target.Length0;

        public int Length1 => Target.Length1;

        public int GetLength(int dim) => Target.GetLength(dim);

        /// <summary>
        /// <inheritdoc cref="ItemGetDelegate"/>
        /// Refer to <seealso cref="ItemGetDelegate"/> for details.
        /// </summary>
        public Func<UT, T> GetFunc { get; }

        /// <summary>
        /// <inheritdoc cref="ItemUpdateDelegate"/>
        /// Refer to <seealso cref="ItemUpdateDelegate"/> for details.
        /// </summary>
        public Func<UT, T, UT> UpdateFunc { get; }

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
                UT oldValue = Target[idx0, idx1];
                Target[idx0, idx1] = UpdateFunc(oldValue, value);
            }
        }

        public Array2HeteroHook(IArray2<UT> target, Func<UT, T> getFunc, Func<UT, T, UT> updateFunc)
        {
            Target = target;
            GetFunc = getFunc;
            UpdateFunc = updateFunc;
        }

        public Array2HeteroHook(IArray2<UT> target, ItemGetDelegate getFunc, ItemUpdateDelegate updateFunc)
        {
            Target = target;
            GetFunc = new Func<UT, T>(getFunc);
            UpdateFunc = new Func<UT, T, UT>(updateFunc);
        }
    }
}
