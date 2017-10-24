using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System
{
    public abstract class FunctionalEnum<TEnum, TKey> where TEnum : FunctionalEnum<TEnum, TKey>
                                                      where TKey : struct
    {
        public readonly TKey Key;
        public readonly string Name;
        public FunctionalEnum(TKey key, string name)
        {
            Key = key;
            Name = name;
            enumValues.Add((TEnum)this);
        }
        public override string ToString()
        {
            return Name;
        }
        protected static List<TEnum> enumValues = new List<TEnum>();
        protected static TEnum GetBaseByKey(TKey key)
        {
            foreach (TEnum t in enumValues)
                if (t.Key.Equals(key))
                    return t;
            return null;
        }
        protected static ReadOnlyCollection<TEnum> GetBaseValues()
        {
            return enumValues.AsReadOnly();
        }
    }
}