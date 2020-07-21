using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Caching
{
    /// <summary>
    /// An adapter that implements <see cref="IItemSource{T}"/>.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ItemSource<T>
        : IItemSource<T>
    {
        #region private
        private Func<int, T> _getter;
        private ItemFactory<T> _factory;
        private IReadOnlyList<T> _list;
        private IReadOnlyDictionary<int, T> _dict;
        #endregion

        public Func<int, T> Getter
        {
            get => _getter;
            set => AssignGetter(value);
        }

        public ItemFactory<T> Factory
        {
            get => _factory;
            set => AssignFactory(value);
        }

        public IReadOnlyList<T> List
        {
            get => _list;
            set => AssignList(value);
        }

        public IReadOnlyDictionary<int, T> Dictionary
        {
            get => _dict;
            set => AssignDict(value);
        }

        public T this[int key]
        {
            get
            {
                if (Getter is null)
                {
                    return default;
                }
                return Getter(key);
            }
        }

        public ItemSource()
        { 
        }

        public ItemSource(Func<int, T> getter)
            : this()
        {
            AssignGetter(getter);
        }

        public ItemSource(ItemFactory<T> factory)
            : this()
        {
            AssignFactory(factory);
        }

        public ItemSource(IReadOnlyList<T> list)
            : this()
        {
            AssignList(list);
        }

        public ItemSource(IReadOnlyDictionary<int, T> dict)
            : this()
        {
            AssignDict(dict);
        }

        /// <summary>
        /// Specify a getter function as the sole access method. 
        /// </summary>
        /// <param name="getter"></param>
        public void AssignGetter(Func<int, T> getter)
        {
            if (getter is null)
            {
                DetachAll();
                return;
            }
            // To avoid confusion, the other access methods are detached.
            _getter = getter;
            _factory = null;
            _list = null;
            _dict = null;
        }

        /// <summary>
        /// Specify an ItemFactory as the sole access method.
        /// </summary>
        /// <param name="factory"></param>
        public void AssignFactory(ItemFactory<T> factory)
        {
            if (factory is null)
            {
                DetachAll();
                return;
            }
            // To avoid confusion, the other access methods are updated or detached.
            _factory = factory;
            _getter = (int key) => factory.Get(key);
            _list = null;
            _dict = null;
        }

        /// <summary>
        /// Specify a <see cref="List"/> or <see cref="IReadOnlyList{T}"/> as the sole access method.
        /// </summary>
        /// <param name="list"></param>
        public void AssignList(IReadOnlyList<T> list)
        {
            if (list is null)
            {
                DetachAll();
                return;
            }
            // To avoid confusion, the other access methods are updated or detached.
            _list = list;
            _getter = (int key) => list[key];
            _factory = null;
            _dict = null;
        }

        public void AssignDict(IReadOnlyDictionary<int, T> dict)
        {
            if (dict is null)
            {
                DetachAll();
                return;
            }
            // To avoid confusion, the other access methods are updated or detached.
            _dict = dict;
            _getter = (int key) => dict[key];
            _list = null;
            _factory = null;
        }

        public void DetachAll()
        {
            _getter = null;
            _factory = null;
            _list = null;
            _dict = null;
        }
    }
}
