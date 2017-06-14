using System;

namespace TitanBotBase.Dependencies
{
    public interface IDependencyFactory : IDisposable, IInstanceBuilder
    {
        void Store<T>(T value);
        void Store(Type type, object value);
        void Store(params object[] values);
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
        bool TryConstructAndStore<T>(out T obj);
        bool TryConstructAndStore<T>(out T obj, params Type[] pattern);
        bool TryConstructAndStore(Type type, out object obj);
        bool TryConstructAndStore(Type type, out object obj, params Type[] pattern);
        T ConstructAndStore<T>();
        T ConstructAndStore<T>(params Type[] pattern);
        object ConstructAndStore(Type type);
        object ConstructAndStore(Type type, params Type[] pattern);
    }
}
