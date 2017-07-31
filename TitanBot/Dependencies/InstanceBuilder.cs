using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TitanBot.Util;

namespace TitanBot.Dependencies
{
    public class InstanceBuilder : IInstanceBuilder
    {
        private readonly Dictionary<Type, object> Store;
        private readonly Dictionary<Type, Func<object>> Builders;
        private readonly Dictionary<Type, Type> TypeMap;
        private Type[] KnownTypes => Store.Keys.Cast<Type>().Concat(Builders.Keys).Distinct().ToArray();

        private Dictionary<Type, object> ParentStore;
        private Dictionary<Type, Func<object>> ParentBuilders;

        private static readonly BindingFlags CtorFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

        public InstanceBuilder(Dictionary<Type, object> parentStore, Dictionary<Type, Func<object>> parentBuilders, Dictionary<Type, Type> parentMap)
        {
            Store = parentStore.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            ParentStore = parentStore;
            Builders = parentBuilders.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            ParentBuilders = parentBuilders;
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
            if (Store.TryGetValue(type, out result))
                return true;
            foreach (var key in Store.Keys)
                if (type.IsAssignableFrom(key) || type.IsInterface && key.GetInterfaces().Contains(type))
                    return Store.TryGetValue(key, out result);
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

        public bool TryGetOrConstruct<T>(out T result)
        {
            result = default(T);
            var res = TryGetOrConstruct(typeof(T), out object a);
            if (res)
                result = (T)a;
            return res;
        }
        public bool TryGetOrConstruct(Type type, out object result)
        {
            if (TryGet(type, out result))
                return true;
            return TryConstruct(type, out result);
        }

        public T GetOrConstruct<T>()
            => (T)GetOrConstruct(typeof(T));
        public object GetOrConstruct(Type type)
        {
            if (TryGet(type, out object result))
                return result;
            return Construct(type);
        }

        public bool TryGetOrStore<T>(out T result)
        {
            result = default(T);
            var res = TryGetOrStore(typeof(T), out object a);
            if (res)
                result = (T)a;
            return res;
        }
        public bool TryGetOrStore(Type type, out object result)
        {
            if (TryGet(type, out result))
                return true;
            if (TryConstruct(type, out result))
            {
                ParentStore[type] = result;
                return true;
            }
            return false;
        }

        public T GetOrStore<T>()
            => (T)GetOrStore(typeof(T));
        public object GetOrStore(Type type)
        {
            if (TryGet(type, out object result))
                return result;
            var res = Construct(type);
            ParentStore[type] = res;
            return res;
        }

        public IInstanceBuilder WithInstance<T>(T value)
            => WithInstance(typeof(T), value);

        public IInstanceBuilder WithInstance(Type type, object value)
            => MiscUtil.InlineAction(this, o => o.Store[type] = value);

        public IInstanceBuilder WithInstanceBuilder<T>(Func<T> builder)
            => WithInstanceBuilder(typeof(T), () => builder());

        public IInstanceBuilder WithInstanceBuilder(Type type, Func<object> builder)
            => MiscUtil.InlineAction(this, o => o.Builders[type] = builder);
    }
}
