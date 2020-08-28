using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrollStitch.V20200707.Collections
{
    public static class ReadOnlyUtility
    {
        public static ReadOnlyDictionary<TK, TV> AsReadOnly<TK, TV>(this Dictionary<TK, TV> dict)
        {
            return new ReadOnlyDictionary<TK, TV>(dict);
        }

        public static Dictionary<TK2, TV> ConvertKeys<TK, TV, TK2>(this Dictionary<TK, TV> dict, Func<TK, TK2> keyConvertFunc)
        {
            var dict2 = new Dictionary<TK2, TV>();
            foreach (var kvp in dict)
            {
                dict2.Add(keyConvertFunc(kvp.Key), kvp.Value);
            }
            return dict2;
        }

        public static Dictionary<TK, TV2> ConvertValues<TK, TV, TV2>(this Dictionary<TK, TV> dict, Func<TV, TV2> valueConvertFunc)
        {
            var dict2 = new Dictionary<TK, TV2>();
            foreach (var kvp in dict)
            {
                dict2.Add(kvp.Key, valueConvertFunc(kvp.Value));
            }
            return dict2;
        }

        public static Dictionary<TK2, TV2> ConvertKeysAndValues<TK, TV, TK2, TV2>(this Dictionary<TK, TV> dict, Func<TK, TK2> keyConvertFunc, Func<TV, TV2> valueConvertFunc)
        {
            var dict2 = new Dictionary<TK2, TV2>();
            foreach (var kvp in dict)
            {
                dict2.Add(keyConvertFunc(kvp.Key), valueConvertFunc(kvp.Value));
            }
            return dict2;
        }

        public static ReadOnlyDictionary<TK, IReadOnlyList<TV>> AsReadOnly<TK, TV>(this Dictionary<TK, List<TV>> dictOfList)
        {
            IReadOnlyList<TV> listRoFunc(List<TV> list)
            {
                return list.AsReadOnly();
            }
            return dictOfList.ConvertValues(listRoFunc).AsReadOnly();
        }
    }
}
