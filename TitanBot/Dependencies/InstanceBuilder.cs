using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TitanBot.Dependencies
{
    public class InstanceBuilder : IInstanceBuilder
    {
        private readonly Dictionary<Type, object> BuildingObjects;
        private readonly Dictionary<Type, Type> TypeMap;
        private Type[] KnownTypes => BuildingObjects.Keys.Cast<Type>().ToArray();

        private static readonly BindingFlags CtorFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public InstanceBuilder(Dictionary<Type, object> parentStore, Dictionary<Type, Type> parentMap)
        {
            BuildingObjects = parentStore.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            TypeMap = parentMap;
        }

        bool TryGet<T>(out T result)
        {
            if (TryGet(typeof(T), out object res))
            {
                result = (T)res;
                return true;
            }
            result = default(T);
            return false;
        }

        bool TryGet(Type type, out object result)
        {
            if (BuildingObjects.TryGetValue(type, out result))
                return true;
            foreach (var key in KnownTypes)
            {
                if (type.IsAssignableFrom(key))
                    return BuildingObjects.TryGetValue(key, out result);
            }
            return false;
        }

        T Get<T>()
            => (T)Get(typeof(T));

        object Get(Type type)
        {
            if (!TryGet(type, out object obj))
                throw new KeyNotFoundException($"The type {type.Name} has not yet been supplied to the manager");
            return obj;
        }

        public bool TryConstruct<T>(out T obj)
        {
            obj = default(T);
            if (!TryConstruct(typeof(T), out object res) || !(res is T))
                return false;
            obj = (T)res;
            return true;
        }

        public bool TryConstruct(Type type, out object obj)
        {
            obj = null;
            if (!TryFindMapping(type, out Type targetType))
                targetType = type;
            var constructors = targetType.GetConstructors(CtorFlags).ToDictionary(c => c, c => c.GetParameters().Select(p => p.ParameterType).ToArray());
            foreach (var ctor in constructors.OrderByDescending(c => c.Value.Count()))
            {
                if (TryConstruct(targetType, out obj, ctor.Value))
                    return true;
            }
            if (TryConstruct(targetType, out obj, Type.EmptyTypes))
                return true;
            return false;
        }

        private bool TryFindMapping(Type input, out Type mapped)
        {
            mapped = null;
            if (TypeMap.TryGetValue(input, out mapped))
                return true;
            if (input.IsGenericType)
            {
                foreach (var mapping in TypeMap)
                {
                    if (input.GetGenericTypeDefinition() == mapping.Key)
                    {
                        mapped = mapping.Value.MakeGenericType(input.GenericTypeArguments);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool TryConstruct<T>(out T obj, params Type[] pattern)
        {
            obj = default(T);
            if (!TryConstruct(typeof(T), out object res, pattern) || !(res is T))
                return false;
            obj = (T)res;
            return true;
        }

        public bool TryConstruct(Type type, out object obj, params Type[] pattern)
        {
            obj = null;
            if (type.IsAbstract || type.IsInterface || type.IsEnum)
                return false;
            var ctor = type.GetConstructor(CtorFlags, null, pattern, null);
            if (ctor == null)
                return false;
            var args = new List<object>();
            foreach (var param in ctor.GetParameters())
            {
                if (TryGet(param.ParameterType, out object res))
                    args.Add(res);
                else if (param.HasDefaultValue)
                    args.Add(param.DefaultValue);
                else if (param.ParameterType != type && TypeMap.TryGetValue(param.ParameterType, out Type mapped) && TryConstruct(mapped, out res))
                    args.Add(res);
                else
                    return false;
            }
            obj = ctor.Invoke(args.ToArray());
            return true;
        }

        public T Construct<T>()
            => (T)Construct(typeof(T));
        public T Construct<T>(params Type[] pattern)
            => (T)Construct(typeof(T), pattern);

        public object Construct(Type type)
        {
            if (!TryConstruct(type, out object obj))
                throw new EntryPointNotFoundException($"Could not locate a usable constructor on the type {type.Name} given the current known types.");
            return obj;
        }

        public object Construct(Type type, params Type[] pattern)
        {
            if (!TryConstruct(type, out object obj, pattern))
                throw new EntryPointNotFoundException($"Could not locate a usable constructor on the type {type.Name} given the current known types.");
            return obj;
        }

        public IInstanceBuilder WithInstance<T>(T value)
            => WithInstance(typeof(T), value);

        public IInstanceBuilder WithInstance(Type type, object value)
        {
            BuildingObjects[type] = value;
            return this;
        }
    }
}
