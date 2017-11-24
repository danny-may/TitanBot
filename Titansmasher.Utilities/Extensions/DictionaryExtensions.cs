using System.Collections.Generic;

namespace Titansmasher.Extensions
{
    public static class DictionaryExtensions
    {
        public static bool TryGetCast<TKey, TValue, TCast>(this Dictionary<TKey, TValue> dict, TKey key, out TCast value) where TCast : TValue
        {
            if (dict.TryGetValue(key, out var val))
            {
                value = (TCast)val;
                return true;
            }
            value = default;
            return false;
        }
    }
}