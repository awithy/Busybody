using System;
using System.Collections.Generic;
using System.Linq;

namespace Busybody
{
    public static class DictionaryExtensions
    {
        public static bool ContainsKeyIgnoreCase<T>(this Dictionary<string,T> dictionary, string key)
        {
            return dictionary.Keys.Any(x => x.Equals(key, StringComparison.CurrentCultureIgnoreCase));
        }

        public static T GetValueOrNullIgnoreCase<T>(this Dictionary<string,T> dictionary, string key) where T : class
        {
            return dictionary.Keys.Any(x => x.Equals(key, StringComparison.CurrentCultureIgnoreCase)) 
                ? dictionary.Single(x => x.Key.Equals(key, StringComparison.CurrentCultureIgnoreCase)).Value 
                : null;
        }
    }
}