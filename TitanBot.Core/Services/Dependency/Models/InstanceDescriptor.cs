using System;
using TitanBot.Core.Models;

namespace TitanBot.Core.Services.Dependency.Models
{
    public sealed class InstanceDescriptor
    {
        #region Fields

        public InstanceLifetime Lifetime { get; }
        public Type InstanceType { get; }
        public Type ImplimentationType { get; }
        public Optional<object> ImplimentationInstance { get; }
        public Func<IInstanceScope, object> ImplementationFactory { get; }

        #endregion Fields

        #region Constructors

        public InstanceDescriptor(Type instanceType, object instance) : this(instanceType, InstanceLifetime.Singleton)
        {
            ImplimentationInstance = instance;
        }
        public InstanceDescriptor(Type instanceType, Type implementationType, InstanceLifetime lifetime) : this(instanceType, lifetime)
        {
            ImplimentationType = implementationType ?? throw new ArgumentNullException(nameof(implementationType));
        }
        public InstanceDescriptor(Type instanceType, Func<IInstanceScope, object> factory, InstanceLifetime lifetime) : this(instanceType, lifetime)
        {
            ImplementationFactory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        private InstanceDescriptor(Type instanceType, InstanceLifetime lifetime)
        {
            ImplimentationType = InstanceType = instanceType ?? throw new ArgumentNullException(nameof(instanceType));
            Lifetime = lifetime;
        }

        #endregion Constructors
    }
}