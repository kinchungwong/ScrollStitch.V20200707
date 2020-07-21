using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Memory
{
    public class ExactLengthArrayPool<T>
        : IArrayPoolClient<T>
    {
        public static ExactLengthArrayPool<T> DefaultInstance { get; } = new ExactLengthArrayPool<T>();

        public bool SanitizeOnReturn { get; set; }

        private ConcurrentDictionary<int, ConcurrentBag<WeakReference<T[]>>> _bagsByLength =
            new ConcurrentDictionary<int, ConcurrentBag<WeakReference<T[]>>>();

        public ExactLengthArrayPool()
        {
            // SanitizeOnReturn = typeof(T).IsClass;
            SanitizeOnReturn = true;
        }

        public T[] Rent(int length)
        {
            _ValidateLengthElseThrow(length);
            var bag = _GetBagForLength(length);
            while (bag.TryTake(out WeakReference<T[]> result))
            {
                if (result.TryGetTarget(out T[] arr))
                {
                    return arr;
                }
            }
            return new T[length];
        }

        public T[] Rent(int minLength, int maxLength)
        {
            return Rent(minLength);
        }

        public void Return(T[] obj)
        {
            if (obj is null)
            {
                return;
            }
            int len = obj.Length;
            if (SanitizeOnReturn)
            {
                Array.Clear(obj, 0, len);
            }
            var bag = _GetBagForLength(len);
            bag.Add(new WeakReference<T[]>(obj));
        }

        public void Clear()
        {
            _bagsByLength.Clear();
        }

        public void ClearArraysWithLength(int length)
        {
            _ValidateLengthElseThrow(length);
            var bag = _GetBagForLength(length);
            while (bag.TryTake(out var _))
            {
            }
        }

        /// <summary>
        /// Returns the memory usage statistics of the array pool.
        /// 
        /// <para>
        /// Important: this method may interfere with the operations of the array pool. Do not call this 
        /// method while the array pool is in active use, especially in a multithreaded execution 
        /// environment.
        /// </para>
        /// </summary>
        /// 
        /// <returns>
        /// An array of usage statistics. Each record corresponds to a length-specific collection of 
        /// array instances.
        /// </returns>
        /// 
        public Diagnostics.ArrayPoolMemoryUsage Unsafe_GetMemoryUsage()
        {
            return Diagnostics.ArrayPoolMemoryUsage.Create(this, _bagsByLength.ToArray());
        }

        private ConcurrentBag<WeakReference<T[]>> _GetBagForLength(int length)
        {
            return _bagsByLength.AddOrUpdate(
                key: length,
                addValueFactory: (_len) => new ConcurrentBag<WeakReference<T[]>>(),
                updateValueFactory: (_len, _oldValue) => _oldValue);
        }

        private void _ValidateLengthElseThrow(int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(paramName: nameof(length));
            }
        }
    }
}
