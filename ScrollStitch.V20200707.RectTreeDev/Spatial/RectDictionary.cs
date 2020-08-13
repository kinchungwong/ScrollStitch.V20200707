using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Spatial
{
    using ScrollStitch.V20200707.Collections;
    using ScrollStitch.V20200707.Data;
    using RectTreeInternals;

    public class RectDictionary<TKey, TValue>
    {
        public IReadOnlyDictionary<TKey, TValue> Dict { get; }

        public Func<TKey, TValue, Rect> GetRectFunc { get; }

        private List<TKey> _keys;

        public Node _rootNode;

        public RectDictionary(IReadOnlyDictionary<TKey, TValue> dict)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }
            var getRectFunc = DefaultRectFunctionFactory.Create(default(KeyValuePair<TKey, TValue>));
            if (getRectFunc is null)
            {
                throw new Exception(
                    "No natural mapping from KeyValuePair<" + typeof(TKey).Name + ", " +
                    typeof(TValue).Name + "> to Rect.");
            }
            Dict = dict;
            GetRectFunc = getRectFunc;
        }

        public RectDictionary(IReadOnlyDictionary<TKey, TValue> dict, Func<TKey, TValue, Rect> getRectFunc)
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }
            if (getRectFunc is null)
            {
                throw new ArgumentNullException(nameof(getRectFunc));
            }
            Dict = dict;
            GetRectFunc = getRectFunc;
        }

        public void Process()
        {
            const int boundRadius = 1024 * 1024;
            Rect boundsRect = new Rect(-boundRadius, -boundRadius, 2 * boundRadius, 2 * boundRadius);
            var settings = new NodeSettings();
            _keys = new List<TKey>();
            _rootNode = new Node(new NodeBounds(boundsRect), settings);
            foreach (KeyValuePair<TKey, TValue> item in Dict)
            {
                _keys.Add(item.Key);
                int intKey = _keys.Count - 1;
                Rect rect = GetRectFunc(item.Key, item.Value);
                _rootNode.Add(rect, intKey);
            }
        }

        public void ForEach(Rect queryRect, Action<TKey, TValue, Rect> action)
        { 
            void itemAction(Rect rect, int intKey)
            {
                TKey key = _keys[intKey];
                TValue value = Dict[key];
                action(key, value, rect);
            }
            _rootNode.Query(queryRect, itemAction);
        }

        public void ForEach(Rect queryRect, Action<TKey, TValue> action)
        {
            void itemAction(Rect rect, int intKey)
            {
                TKey key = _keys[intKey];
                TValue value = Dict[key];
                action(key, value);
            }
            _rootNode.Query(queryRect, itemAction);
        }

        public void ForEach(Rect queryRect, Action<KeyValuePair<TKey, TValue>, Rect> action)
        {
            void itemAction(Rect rect, int intKey)
            {
                TKey key = _keys[intKey];
                TValue value = Dict[key];
                action(new KeyValuePair<TKey, TValue>(key, value), rect);
            }
            _rootNode.Query(queryRect, itemAction);
        }

        public void ForEach(Rect queryRect, Action<KeyValuePair<TKey, TValue>> action)
        {
            void itemAction(Rect rect, int intKey)
            {
                TKey key = _keys[intKey];
                TValue value = Dict[key];
                action(new KeyValuePair<TKey, TValue>(key, value));
            }
            _rootNode.Query(queryRect, itemAction);
        }

        public static class DefaultRectFunctionFactory
        {
            public static Func<Rect, TValue_0, Rect> Create<TValue_0>(KeyValuePair<Rect, TValue_0> unused = default)
            {
                return (Rect r, TValue_0 v) => r;
            }

            public static Func<TKey_0, Rect, Rect> Create<TKey_0>(KeyValuePair<TKey_0, Rect> unused = default)
            {
                return (TKey_0 k, Rect r) => r;
            }

            public static Func<TKey_0, TValue_0, Rect> Create<TKey_0, TValue_0>(KeyValuePair<TKey_0, TValue_0> unused = default)
            {
                Rect TakeKeyAsRect(TKey_0 k, TValue_0 v)
                {
                    switch (k)
                    {
                        case Rect r:
                            return r;
                        default:
                            return default;
                    }
                }
                Rect TakeValueAsRect(TKey_0 k, TValue_0 v)
                {
                    switch (v)
                    {
                        case Rect r:
                            return r;
                        default:
                            return default;
                    }
                }
                switch (unused)
                {
                    case KeyValuePair<Rect, TValue_0> _:
                        return TakeKeyAsRect;
                    case KeyValuePair<TKey_0, Rect> _:
                        return TakeValueAsRect;
                    default:
                        return null;
                }
            }
        }
    }
}
