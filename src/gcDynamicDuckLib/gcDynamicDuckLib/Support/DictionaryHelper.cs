using System;
using System.Collections.Generic;
using System.Linq;

namespace GeniusCode.Components.DynamicDuck.Support
{
    internal static class DictionaryHelper<TKey, TValue>
    {

        public static void CopyContents(Dictionary<TKey, TValue> fromDictionary, Dictionary<TKey, TValue> toDictionary)
        {
            Array.ForEach(fromDictionary.ToArray(), pair =>
            {
                if (!toDictionary.ContainsKey(pair.Key))
                    toDictionary.Add(pair.Key, pair.Value);
            });
        }
        /// <summary>
        /// In subtraction, a subtrahend is subtracted from a minuend to find a difference
        /// </summary>
        /// <param name="minuend">the original collection</param>
        /// <param name="subtrahend">what is subtracted</param>
        public static void SubtractContents(Dictionary<TKey, TValue> minuend, Dictionary<TKey, TValue> subtrahend)
        {
            Array.ForEach(subtrahend.ToArray(), pair =>
            {
                if (minuend.ContainsKey(pair.Key))
                    minuend.Remove(pair.Key);
            });
        }

    }
}
