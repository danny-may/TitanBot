using System;
using TitanBot.Core.Services.Dependency;
using TitanBot.Core.Services.Dependency.Models;

namespace TitanBot.Services.Dependency
{
    public class InstanceProvider : InstanceScope, IInstanceProvider
    {
        #region Fields

        public IInstanceProvider ParentProvider { get; }

        #endregion Fields

        #region Constructors

        public InstanceProvider()
        {
        }

        private InstanceProvider(InstanceProvider parent) : base(parent)
        {
            ParentProvider = parent;
        }

        #endregion Constructors

        #region Methods

        private InvalidOperationException UnableToAdd(InstanceDescriptor descriptor)
            => new InvalidOperationException($"Unable to add descriptor for type {descriptor.InstanceType}");

        #endregion Methods

        #region IInstanceProvider

        public IInstanceProvider Add(InstanceDescriptor descriptor)
            => _descriptors.TryAdd(descriptor.InstanceType, descriptor) ? this : throw UnableToAdd(descriptor);

        #region Scoped

        public IInstanceProvider AddScoped(Type instanceType)
            => AddScoped(instanceType, instanceType);

        public IInstanceProvider AddScoped(Type instanceType, Type implimentationType)
            => Add(new InstanceDescriptor(instanceType, implimentationType, InstanceLifetime.Scoped));

        public IInstanceProvider AddScoped(Type instanceType, Func<IInstanceScope, object> implimentationFactory)
            => Add(new InstanceDescriptor(instanceType, implimentationFactory, InstanceLifetime.Scoped));

        public IInstanceProvider AddScoped<TInstance>()
            => AddScoped<TInstance, TInstance>();

        public IInstanceProvider AddScoped<TInstance, TImplimentation>() where TImplimentation : TInstance
            => AddScoped(typeof(TInstance), typeof(TImplimentation));

        public IInstanceProvider AddScoped<TInstance>(Func<IInstanceScope, TInstance> implimentationFactory)
            => AddScoped<TInstance, TInstance>(implimentationFactory);

        public IInstanceProvider AddScoped<TInstance, TImplimentation>(Func<IInstanceScope, TImplimentation> implimentationFactory) where TImplimentation : TInstance
            => AddScoped(typeof(TInstance), s => implimentationFactory(s));

        #endregion Scoped

        #region Singleton

        public IInstanceProvider AddSingleton(Type instanceType)
            => AddSingleton(instanceType, instanceType);

        public IInstanceProvider AddSingleton(Type instanceType, Type implimentationType)
            => Add(new InstanceDescriptor(instanceType, implimentationType, InstanceLifetime.Singleton));

        public IInstanceProvider AddSingleton(Type instanceType, Func<IInstanceScope, object> implimentationFactory)
            => Add(new InstanceDescriptor(instanceType, implimentationFactory, InstanceLifetime.Singleton));

        public IInstanceProvider AddSingleton<TInstance>()
            => AddSingleton<TInstance, TInstance>();

        public IInstanceProvider AddSingleton<TInstance, TImplimentation>() where TImplimentation : TInstance
            => AddSingleton(typeof(TInstance), typeof(TImplimentation));

        public IInstanceProvider AddSingleton<TInstance>(Func<IInstanceScope, TInstance> implimentationFactory)
            => AddSingleton<TInstance, TInstance>(implimentationFactory);

        public IInstanceProvider AddSingleton<TInstance, TImplimentation>(Func<IInstanceScope, TImplimentation> implimentationFactory) where TImplimentation : TInstance
            => AddSingleton(typeof(TInstance), s => implimentationFactory(s));

        public IInstanceProvider AddSingleton<TInstance>(TInstance instance)
            => AddSingleton<TInstance, TInstance>(instance);

        public IInstanceProvider AddSingleton<TInstance, TImplimentation>(TImplimentation instance) where TImplimentation : TInstance
            => Add(new InstanceDescriptor(typeof(TInstance), instance));

        #endregion Singleton

        #region Transient

        public IInstanceProvider AddTransient(Type instanceType)
            => AddTransient(instanceType, instanceType);

        public IInstanceProvider AddTransient(Type instanceType, Type implimentationType)
            => Add(new InstanceDescriptor(instanceType, implimentationType, InstanceLifetime.Transient));

        public IInstanceProvider AddTransient(Type instanceType, Func<IInstanceScope, object> implimentationFactory)
            => Add(new InstanceDescriptor(instanceType, implimentationFactory, InstanceLifetime.Transient));

        public IInstanceProvider AddTransient<TInstance>()
            => AddTransient<TInstance, TInstance>();

        public IInstanceProvider AddTransient<TInstance, TImplimentation>() where TImplimentation : TInstance
            => AddTransient(typeof(TInstance), typeof(TImplimentation));

        public IInstanceProvider AddTransient<TInstance>(Func<IInstanceScope, TInstance> implimentationFactory)
            => AddTransient<TInstance, TInstance>(implimentationFactory);

        public IInstanceProvider AddTransient<TInstance, TImplimentation>(Func<IInstanceScope, TImplimentation> implimentationFactory) where TImplimentation : TInstance
            => AddTransient(typeof(TInstance), s => implimentationFactory(s));

        #endregion Transient

        public IInstanceProvider CreateChild()
            => new InstanceProvider(this);

        #endregion IInstanceProvider
    }
}