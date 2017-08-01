using System;
using System.Collections.Generic;

namespace TitanBot.Dependencies
{
    public interface IDependencyFactory : IDisposable, IInstanceBuilder
    {
        List<Type> History { get; }

        void Store<T>(T value);
        void Store(Type type, object value);
        void StoreBuilder<T>(Func<T> builder);
        void StoreBuilder(Type type, Func<object> builder);

        void Map<From, To>()
            where To : From;
        void Map(Type from, Type to);
        bool TryMap<From, To>()
            where To : From;
        bool TryMap(Type from, Type to);

        bool TryGet<T>(out T result);
        bool TryGet(Type type, out object result);
        T Get<T>();
        object Get(Type type);
    }
}
