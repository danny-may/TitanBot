using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot.Dependencies
{
    public partial class DependencyFactory : IDependencyFactory
    {
        private readonly Dictionary<Type, object> Stored = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Func<object>> Builders = new Dictionary<Type, Func<object>>();
        private readonly Dictionary<Type, Type> TypeMap = new Dictionary<Type, Type>();
        private Type[] KnownTypes => Stored.Keys.Cast<Type>().Concat(Builders.Keys).Distinct().ToArray();

        public List<Type> History { get; } = new List<Type>();

        public DependencyFactory()
        {
            Store(this);
        }

        public void Dispose()
        {
            Stored.Clear();
        }

        public void Store<T>(T value)
            => Store(typeof(T), value);
        public void Store(Type type, object value)
            => Stored.Add(type, value);
        public void StoreBuilder<T>(Func<T> builder)
            => StoreBuilder(typeof(T), () => builder());
        public void StoreBuilder(Type type, Func<object> builder)
            => Builders.Add(type, builder);

        public void Map<From, To>() where To : From
            => Map(typeof(From), typeof(To));
        public void Map(Type from, Type to)
            => TypeMap[from] = to;

        public bool TryMap<From, To>() where To : From
            => TryMap(typeof(From), typeof(To));

        public bool TryMap(Type from, Type to)
        {
            if (TypeMap.ContainsKey(from))
                return false;
            Map(from, to);
            return true;
        }

        public bool TryGet<T>(out T result)
        {
            result = default(T);
            var res = TryGet(typeof(T), out object a);
            if (res)
                result = (T)a;
            return res;
        }
        public bool TryGet(Type type, out object result)
        {
            if (Stored.TryGetValue(type, out result))
                return true;
            foreach (var key in Stored.Keys)
                if (type.IsAssignableFrom(key) || type.IsInterface && key.GetInterfaces().Contains(type))
                    return Stored.TryGetValue(key, out result);
            if (Builders.TryGetValue(type, out var builder))
            {
                result = builder();
                return true;
            }
            foreach (var key in Builders.Keys)
                if (type.IsAssignableFrom(key) || type.IsInterface && key.GetInterfaces().Contains(type))
                {
                    Builders.TryGetValue(key, out builder);
                    result = builder();
                    return true;
                }
            return false;
        }
        public T Get<T>()
            => (T)Get(typeof(T));
        public object Get(Type type)
            => TryGet(type, out var result) ? result : throw new KeyNotFoundException($"Could not locate a stored object of type {type}");

        IInstanceBuilder GetBuilder()
            => new InstanceBuilder(this);

        public IInstanceBuilder WithInstance<T>(T value)
            => GetBuilder().WithInstance(value);
        public IInstanceBuilder WithInstance(Type type, object value)
            => GetBuilder().WithInstance(type, value);
        public IInstanceBuilder WithInstanceBuilder<T>(Func<T> builder)
            => GetBuilder().WithInstanceBuilder(builder);
        public IInstanceBuilder WithInstanceBuilder(Type type, Func<object> builder)
            => GetBuilder().WithInstanceBuilder(type, builder);

        public bool TryConstruct<T>(out T obj)
            => GetBuilder().TryConstruct(out obj);
        public bool TryConstruct(Type type, out object obj)
            => GetBuilder().TryConstruct(type, out obj);
        public T Construct<T>()
            => GetBuilder().Construct<T>();
        public object Construct(Type type)
            => GetBuilder().Construct(type);

        public bool TryGetOrConstruct<T>(out T result)
            => GetBuilder().TryGetOrConstruct(out result);
        public bool TryGetOrConstruct(Type type, out object result)
            => GetBuilder().TryGetOrConstruct(type, out result);
        public T GetOrConstruct<T>()
            => GetBuilder().GetOrConstruct<T>();
        public object GetOrConstruct(Type type)
            => GetBuilder().GetOrConstruct(type);

        public bool TryGetOrStore<T>(out T result)
            => GetBuilder().TryGetOrStore(out result);
        public bool TryGetOrStore(Type type, out object result)
            => GetBuilder().TryGetOrStore(type, out result);
        public T GetOrStore<T>()
            => GetBuilder().GetOrStore<T>();
        public object GetOrStore(Type type)
            => GetBuilder().GetOrStore(type);
    }
}
