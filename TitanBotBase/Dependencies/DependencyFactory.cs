using System;
using System.Collections.Generic;
using System.Linq;
using TitanBotBase.Util;

namespace TitanBotBase.Dependencies
{
    public class DependencyFactory : IDependencyFactory
    {
        private readonly Dictionary<Type, object> _objStore = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Type> _map = new Dictionary<Type, Type>();
        private Type[] _availableTypes => _objStore.Keys.Cast<Type>().ToArray();

        public DependencyFactory()
        {
            Store(this);
        }

        public void Store<T>(T value)
            => Store(typeof(T), value);
        public void Store(Type type, object value)
            => _objStore.Add(type, value);
        public void Store(params object[] values)
            => values.ForEach(v => Store(v.GetType(), v));

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
            if (_objStore.TryGetValue(type, out result))
                return true;
            foreach (var key in _availableTypes)
            {
                if (type.IsAssignableFrom(key))
                    return _objStore.TryGetValue(key, out result);
            }
            return false;
        }

        public void Dispose()
        {
            _objStore.Clear();
        }

        public T Get<T>()
            => (T)Get(typeof(T));

        public object Get(Type type)
        {
            if (!TryGet(type, out object obj))
                throw new KeyNotFoundException($"The type {type.Name} has not yet been supplied to the manager");
            return obj;
        }

        IInstanceBuilder GetBuilder()
            => new InstanceBuilder(_objStore, _map);

        public IInstanceBuilder WithInstance<T>(T value)
            => GetBuilder().WithInstance(value);
        public IInstanceBuilder WithInstance(Type type, object value)
            => GetBuilder().WithInstance(type, value);

        public bool TryConstruct<T>(out T obj)
            => GetBuilder().TryConstruct(out obj);
        public bool TryConstruct<T>(out T obj, params Type[] pattern)
            => GetBuilder().TryConstruct(out obj, pattern);
        public bool TryConstruct(Type type, out object obj)
            => GetBuilder().TryConstruct(type, out obj);
        public bool TryConstruct(Type type, out object obj, params Type[] pattern)
            => GetBuilder().TryConstruct(type, out obj, pattern);

        public T Construct<T>()
            => GetBuilder().Construct<T>();
        public T Construct<T>(params Type[] pattern)
            => GetBuilder().Construct<T>(pattern);
        public object Construct(Type type)
            => GetBuilder().Construct(type);
        public object Construct(Type type, params Type[] pattern)
            => GetBuilder().Construct(type, pattern);

        public void Map<From, To>() where To : From
            => Map(typeof(From), typeof(To));
        public void Map(Type from, Type to)
        {
            _map[from] = to;
        }

        public bool TryMap<From, To>() where To : From
            => TryMap(typeof(From), typeof(To));
        public bool TryMap(Type from, Type to)
        {
            if (!_map.ContainsKey(from))
            {
                Map(from, to);
                return true;
            }
            return false;
        }

        public bool TryConstructAndStore<T>(out T obj)
        {
            var success = TryConstruct(out obj);
            if (success)
                Store(obj);
            return success;
        }

        public bool TryConstructAndStore<T>(out T obj, params Type[] pattern)
        {
            var success = TryConstruct(out obj, pattern);
            if (success)
                Store(obj);
            return success;
        }

        public bool TryConstructAndStore(Type type, out object obj)
        {
            var success = TryConstruct(type, out obj);
            if (success)
                Store(obj);
            return success;
        }

        public bool TryConstructAndStore(Type type, out object obj, params Type[] pattern)
        {
            var success = TryConstruct(type, out obj, pattern);
            if (success)
                Store(obj);
            return success;
        }

        public T ConstructAndStore<T>()
            => (T)ConstructAndStore(typeof(T));

        public T ConstructAndStore<T>(params Type[] pattern)
            => (T)ConstructAndStore(typeof(T), pattern);

        public object ConstructAndStore(Type type)
        {
            var res = Construct(type);
            Store(res);
            return res;
        }

        public object ConstructAndStore(Type type, params Type[] pattern)
        {
            var res = Construct(type, pattern);
            Store(res);
            return res;
        }
    }
}
