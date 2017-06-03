using DiscordBot.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Dependencies
{
    public class DependencyManager : IDependencyManager
    {
        private readonly Dictionary<Type, object> _map = new Dictionary<Type, object>();
        private Type[] _availableTypes => _map.Keys.Cast<Type>().ToArray();

        public DependencyManager()
        {
            Add(this);
        }

        public void Add<T>(T value)
            => Add(typeof(T), value);
        public void Add(Type type, object value)
            => _map.Add(type, value);

        public bool TryGet<T>(out T result)
        {
            if (TryGet(typeof(T), out object res))
            {
                result = (T)res;
                return true;
            }
            result = default(T);
            return false;
        }

        public bool TryGet(Type type, out object result)
        {
            if (_map.TryGetValue(type, out result))
                return true;
            foreach (var key in _availableTypes)
            {
                if (type.IsAssignableFrom(key))
                    return _map.TryGetValue(key, out result);
            }
            return false;
        }

        public bool TryConstruct<T>(out T obj)
        {
            obj = default(T);
            var type = typeof(T);
            var constructors = type.GetConstructors().ToDictionary(c => c, c => c.GetParameters().Select(p => p.ParameterType).ToArray());
            foreach (var ctor in constructors.OrderByDescending(c => c.Value.Count()))
            {
                if (TryConstruct(out obj, ctor.Value))
                    return true;
            }
            return false;
        }

        public bool TryConstruct<T>(out T obj, params Type[] pattern)
        {
            obj = default(T);
            var ctor = typeof(T).GetConstructor(pattern);
            if (ctor == null)
                return false;
            var args = new List<object>();
            foreach (var param in ctor.GetParameters())
            {
                if (TryGet(param.ParameterType, out object res))
                    args.Add(res);
                else if (param.HasDefaultValue)
                    args.Add(param.DefaultValue);
                else
                    return false;
            }
            obj = (T)ctor.Invoke(args.ToArray());
            return true;
        }

        public void Dispose()
        {
            _map.Clear();
        }
    }
}
