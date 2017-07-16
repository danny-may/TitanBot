using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot.Dependencies
{
    public class DependencyFactory : IDependencyFactory
    {
        private readonly Dictionary<Type, object> Stored = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Type> TypeMap = new Dictionary<Type, Type>();
        private Type[] KnownTypes => Stored.Keys.Cast<Type>().ToArray();

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
            => Stored.TryGetValue(type, out result);
        public T Get<T>()
            => (T)Get(typeof(T));
        public object Get(Type type)
            => Stored[type];

        IInstanceBuilder GetBuilder()
            => new InstanceBuilder(Stored, TypeMap);

        public IInstanceBuilder WithInstance<T>(T value)
            => GetBuilder().WithInstance(value);
        public IInstanceBuilder WithInstance(Type type, object value)
            => GetBuilder().WithInstance(type, value);

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
