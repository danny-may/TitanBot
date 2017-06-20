using System;

namespace TitanBot.Dependencies
{
    public interface IInstanceBuilder
    {
        IInstanceBuilder WithInstance<T>(T value);
        IInstanceBuilder WithInstance(Type type, object value);

        bool TryConstruct<T>(out T obj);
        bool TryConstruct<T>(out T obj, params Type[] pattern);
        bool TryConstruct(Type type, out object obj);
        bool TryConstruct(Type type, out object obj, params Type[] pattern);
        T Construct<T>();
        T Construct<T>(params Type[] pattern);
        object Construct(Type type);
        object Construct(Type type, params Type[] pattern);
    }
}
