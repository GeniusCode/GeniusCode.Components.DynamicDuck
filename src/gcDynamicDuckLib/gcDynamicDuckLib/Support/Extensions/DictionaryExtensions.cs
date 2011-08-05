using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GeniusCode.Components.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue CreateOrGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createDelegate)
        {
            bool wasCreated;
            return CreateOrGetValue<TKey, TValue>(dictionary, key, createDelegate, out wasCreated);
        }
        public static TValue CreateOrGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> createDelegate, out bool wasCreated)
        {
            TValue output;
            if (dictionary.TryGetValue(key, out output))
            {
                wasCreated = false;
                return output;
            }
            else
            {
                TValue result = createDelegate();
                dictionary.Add(key, result);
                wasCreated = true;
                return result;
            }
        }
    }
}
