using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Caching
{
    /// <summary>
    /// A simplistic item-caching collection.
    /// 
    /// <para>
    /// Internally, it is a Dictionary with integer key and generic item-type, with two functions 
    /// already hooked up: (1) an auto-create function, (2) an item disposal or recycling function.
    /// </para>
    /// 
    /// <para>
    /// Items can be added to the collection anytime.
    /// </para>
    /// 
    /// <para>
    /// All integer values are allowed as key. In particular, negative and non-sequential keys are 
    /// allowed. For this reason, this class does not implement any of the standard collections 
    /// interface.
    /// </para>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemFactory<T>
        : IItemSource<T>
    {
        #region private
        private readonly ConcurrentDictionary<int, T> _items = new ConcurrentDictionary<int, T>();
        #endregion

        /// <summary>
        /// A user-configurable function to be called when an item is to be created.
        /// </summary>
        public Func<int, T> FactoryFunc { get; set; }

        /// <summary>
        /// A function to be called when an item is removed. This function is optional.
        /// 
        /// The implementation of this function must not throw any exception in case the 
        /// item is null.
        /// </summary>
        public Action<int, T> DisposeFunc { get; set; }

        /// <summary>
        /// At runtime, this flag determines the behavior of the <see cref="Get(int)"/> method
        /// by providing a value to the <c>autoCreate</c> flag in calling <see cref="Get(int, bool)"/>.
        /// </summary>
        public bool AutoCreate { get; set; } = true;

        /// <summary>
        /// Returns the number of items in the collection.
        /// </summary>
        /// <remarks>
        /// Important remark. Since negative and non-sequential keys are allowed, the key set is 
        /// not necessary equal to the consecutive key range from 0 to ItemCount.
        /// </remarks>
        public int ItemCount => _items.Count;

        /// <summary>
        /// Gets the item with the specified key.
        /// 
        /// <para>
        /// If the item has not been created, this function's behavior at runtime is controlled by
        /// the <see cref="AutoCreate"/> property on this class.
        /// </para>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T this[int key] => Get(key);

        public ItemFactory()
        {
        }

        public ItemFactory(Func<int, T> factoryFunc)
        {
            FactoryFunc = factoryFunc;
        }

        public ItemFactory(Func<int, T> factoryFunc, Action<int, T> disposeFunc)
        {
            FactoryFunc = factoryFunc;
            if (!(disposeFunc is null))
            {
                DisposeFunc = disposeFunc;
            }
        }

        public ItemFactory(Func<int, T> factoryFunc, Action<T> disposeFunc)
        {
            FactoryFunc = factoryFunc;
            if (!(disposeFunc is null)) 
            {
                DisposeFunc = (int _, T t) => disposeFunc(t);
            }
        }

        public T Get(int key)
        {
            return Get(key, autoCreate: AutoCreate);
        }

        public T Get(int key, bool autoCreate)
        {
            if (autoCreate)
            {
                return _items.GetOrAdd(key, FactoryFunc);
            }
            if (!_items.TryGetValue(key, out T value))
            {
                value = default;
            }
            return value;
        }

        public void Remove(int key)
        {
            if (_items.TryRemove(key, out var removed))
            {
                DisposeFunc?.Invoke(key, removed);
            }
        }
    }
}
