using System;
using System.Collections.Generic;

public static class DictionaryExtensions
{

    public static bool RemoveValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
                                                 Predicate<TValue> predicate,
                                                 out TValue value)
    {

        if (dictionary == null || dictionary.Count == 0)
        {
            value = default(TValue);
            return false;
        }

        foreach (var kvp in dictionary)
        {
            if (!predicate(kvp.Value)) continue;
            value = kvp.Value;
            dictionary.Remove(kvp.Key);
            return true;
        }
        value = default(TValue);
        return false;
    }
    public static bool FindValue<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Predicate<TValue> predicate, out TValue value)
    {
        if (dictionary == null || dictionary.Count == 0)
        {
            value = default(TValue);
            return false;
        }


        foreach (var kvp in dictionary)
        {
            if (!predicate(kvp.Value)) continue;

            value = kvp.Value;
            return true;
        }

        value = default(TValue);
        return false;
    }

}
