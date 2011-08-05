using System;
using System.Collections.Generic;
using System.Linq;

namespace GeniusCode.Components.Extensions
{
    public static class StringExtensions
    {
        public static bool ContainsAnyWords(this string value, IEnumerable<string> words)
        {
            return words.Any(a => value.Contains(a));
        }
    }
}
