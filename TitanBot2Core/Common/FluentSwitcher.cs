using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Common
{
    public class FluentSwitcher
    {
        public static FluentSwitcher<TKey, TValue> Switch<TKey, TValue>(TKey key)
            => FluentSwitcher<TKey, TValue>.Switch(key);
    }
    public class FluentSwitcher<TKey, TValue>
    {
        private Dictionary<TKey, Func<TValue>> Cases { get; } = new Dictionary<TKey, Func<TValue>>();
        private Func<TValue> DefaultAction { get; set; }

        private TKey Switching { get; set; }

        public static FluentSwitcher<TKey, TValue> Switch(TKey key)
            => new FluentSwitcher<TKey, TValue> { Switching = key };

        public FluentSwitcher<TKey, TValue> Case(TKey key, Func<TValue> value)
        {
            if (Cases.ContainsKey(key))
                throw new InvalidOperationException("Cannot have duplicate cases for a FluentSwitcher");
            Cases.Add(key, value);
            return this;
        }

        public FluentSwitcher<TKey, TValue> Case(TKey[] keys, Func<TValue> value)
        {
            foreach (var key in keys)
                Case(key, value);
            return this;
        }

        public FluentSwitcher<TKey, TValue> Default(Func<TValue> value)
        {
            if (DefaultAction != null)
                throw new InvalidOperationException("Can only have 1 default per FluentSwitcher");
            DefaultAction = value;
            return this;
        }

        public static implicit operator TValue(FluentSwitcher<TKey, TValue> obj)
        {
            if (obj != null && obj.Switching != null && obj.Cases.ContainsKey(obj.Switching))
                return obj.Cases[obj.Switching]();
            if (obj.DefaultAction != null)
                return obj.DefaultAction();
            return default(TValue);
        }
    }
}
