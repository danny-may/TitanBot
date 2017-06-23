using System;

namespace TitanBot.Dependencies
{
    public interface IInstanceBuilder
    {
        IInstanceBuilder WithInstance<T>(T value);
        IInstanceBuilder WithInstance(Type type, object value);

        bool TryConstruct<T>(out T obj);
        bool TryConstruct(Type type, out object obj);
        T Construct<T>();
        object Construct(Type type);

        bool TryGetOrConstruct<T>(out T result);
        bool TryGetOrConstruct(Type type, out object result);
        T GetOrConstruct<T>();
        object GetOrConstruct(Type type);

        bool TryGetOrStore<T>(out T result);
        bool TryGetOrStore(Type type, out object result);
        T GetOrStore<T>();
        object GetOrStore(Type type);
    }
}
