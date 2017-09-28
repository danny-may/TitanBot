using System;
using System.Collections.Concurrent;
using System.Linq;
using TitanBot.Core.Services.Dependency;
using TitanBot.Core.Services.Dependency.Models;

namespace TitanBot.Services.Dependency
{
    public delegate bool TryGetInstance(Type type, out object instance);

    public class InstanceScope : IInstanceScope
    {
        #region Fields

        public IInstanceScope ParentScope => _parentScope;
        private InstanceScope _parentScope;
        private InstanceBuilder _builder;

        protected internal ConcurrentDictionary<Type, InstanceDescriptor> _descriptors = new ConcurrentDictionary<Type, InstanceDescriptor>();
        private ConcurrentDictionary<InstanceDescriptor, object> _singletons = new ConcurrentDictionary<InstanceDescriptor, object>();
        private ConcurrentDictionary<InstanceDescriptor, object> _scoped = new ConcurrentDictionary<InstanceDescriptor, object>();

        #endregion Fields

        #region Constructors

        public InstanceScope()
        {
            _builder = new InstanceBuilder();
        }

        public InstanceScope(InstanceScope parent) : this()
        {
            _parentScope = parent;
            _singletons = parent._singletons;
        }

        #endregion Constructors

        #region Methods

        private Type[] GetDescribedTypes()
            => _descriptors.Select(d => d.Key.GetType())
                           .Concat(_parentScope?.GetDescribedTypes() ?? new Type[0])
                           .Distinct()
                           .ToArray();

        private bool TryGetDescriptor(Type instanceType, out InstanceDescriptor descriptor)
            => _descriptors.TryGetValue(instanceType, out descriptor) ? true : _parentScope?.TryGetDescriptor(instanceType, out descriptor) ?? false;

        protected InstanceDescriptor GetOrAddDescriptor(Type instanceType)
            => TryGetDescriptor(instanceType, out var desc) ? desc : _descriptors.GetOrAdd(instanceType, t => new InstanceDescriptor(t, t, InstanceLifetime.Transient));

        private bool TryGetInstance(Type instanceType, object[] withObjects, out Type implimentationType, out object instance)
        {
            var descriptor = GetOrAddDescriptor(instanceType);

            implimentationType = descriptor.ImplimentationType ?? descriptor.InstanceType;

            return TryGetAndStore(descriptor, withObjects, out instance);
        }

        private bool TryGetAndStore(InstanceDescriptor descriptor, object[] withObjects, out object instance)
        {
            Func<InstanceScope, ConcurrentDictionary<InstanceDescriptor, object>> selector = null;

            switch (descriptor.Lifetime)
            {
                case InstanceLifetime.Scoped:
                    selector = s => s._scoped;
                    break;
                case InstanceLifetime.Singleton:
                    selector = s => s._singletons;
                    break;
            }

            ConcurrentDictionary<InstanceDescriptor, object> store;
            InstanceScope next = this;

            do
            {
                store = selector?.Invoke(next);
                if (store != null && store.TryGetValue(descriptor, out instance))
                    return true;
                next = next._parentScope;
            } while (store != null && next != null);

            var success = false;

            if (descriptor.ImplimentationInstance.IsSet)
            {
                instance = descriptor.ImplimentationInstance.Value;
                success = true;
            }
            else if (descriptor.ImplementationFactory != null)
            {
                instance = descriptor.ImplementationFactory(this);
                success = true;
            }
            else if (descriptor.ImplimentationType != null)
                success = _builder.TryConstruct(descriptor.ImplimentationType, CreateInstanceGetter(withObjects), out instance);
            else
                success = _builder.TryConstruct(descriptor.InstanceType, CreateInstanceGetter(withObjects), out instance);

            store = selector?.Invoke(this);

            if (success && store != null)
                store[descriptor] = instance;

            return success;
        }

        private TryGetInstance CreateInstanceGetter(object[] withObjects)
        {
            var typeMap = withObjects.Where(o => o != null)
                                     .ToDictionary(o => o.GetType(), o => o);

            bool TryGet(Type type, out object value)
            {
                if (typeMap.TryGetValue(type, out value))
                    return true;
                foreach (var row in typeMap)
                {
                    if (row.Key.IsSubclassOf(type) ||
                        (type.IsInterface && row.Key.GetInterface(type.Name) != null))
                    {
                        value = row.Value;
                        return true;
                    }
                }
                return TryGetInstance(type, withObjects, out _, out value);
            }

            return TryGet;
        }

        private object Default(Type type)
            => type.IsValueType ? Activator.CreateInstance(type) : null;

        private InvalidOperationException UnableToCreate(Type type)
            => new InvalidOperationException($"Unable to create a `{type}` instance as it either has no public constructors or it relies on an instance that is not yet defined");

        #endregion Methods

        #region IInstanceScope

        public IInstanceScope CreateScope()
            => new InstanceScope(this);

        public object GetInstance(Type type, params object[] withObjects)
            => TryGetInstance(type, withObjects, out var impType, out var instance) ? instance : Default(impType);

        public object GetRequiredInstance(Type type, params object[] withObjects)
            => TryGetInstance(type, withObjects, out _, out var instance) ? instance : throw UnableToCreate(type);

        public object GetInstance(Type type)
            => GetInstance(type, new object[0]);

        public TInstance GetInstance<TInstance>(params object[] withObjects)
            => (TInstance)GetInstance(typeof(TInstance), withObjects);

        public TInstance GetInstance<TInstance>()
            => GetInstance<TInstance>(new object[0]);

        public object GetRequiredInstance(Type type)
            => GetRequiredInstance(type, new object[0]);

        public TInstance GetRequiredInstance<TInstance>(params object[] withObjects)
            => (TInstance)GetRequiredInstance(typeof(TInstance), withObjects);

        public TInstance GetRequiredInstance<TInstance>()
            => GetRequiredInstance<TInstance>(new object[0]);

        #endregion IInstanceScope
    }
}