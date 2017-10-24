using System;
using TitanBot.Core.Services.Dependency.Models;

namespace TitanBot.Core.Services.Dependency
{
    public interface IInstanceProvider : IInstanceScope
    {
        IInstanceProvider ParentProvider { get; }

        IInstanceProvider CreateChild();

        IInstanceProvider Add(InstanceDescriptor descriptor);

        IInstanceProvider AddSingleton(Type instanceType);
        IInstanceProvider AddSingleton(Type instanceType, Type implimentationType);
        IInstanceProvider AddSingleton(Type instanceType, Func<IInstanceScope, object> implimentationFactory);
        IInstanceProvider AddSingleton<TInstance>();
        IInstanceProvider AddSingleton<TInstance, TImplimentation>()
            where TImplimentation : TInstance;
        IInstanceProvider AddSingleton<TInstance>(Func<IInstanceScope, TInstance> implimentationFactory);
        IInstanceProvider AddSingleton<TInstance, TImplimentation>(Func<IInstanceScope, TImplimentation> implimentationFactory)
            where TImplimentation : TInstance;
        IInstanceProvider AddSingleton<TInstance>(TInstance instance);
        IInstanceProvider AddSingleton<TInstance, TImplimentation>(TImplimentation instance)
            where TImplimentation : TInstance;

        IInstanceProvider AddScoped(Type instanceType);
        IInstanceProvider AddScoped(Type instanceType, Type implimentationType);
        IInstanceProvider AddScoped(Type instanceType, Func<IInstanceScope, object> implimentationFactory);
        IInstanceProvider AddScoped<TInstance>();
        IInstanceProvider AddScoped<TInstance, TImplimentation>()
            where TImplimentation : TInstance;
        IInstanceProvider AddScoped<TInstance>(Func<IInstanceScope, TInstance> implimentationFactory);
        IInstanceProvider AddScoped<TInstance, TImplimentation>(Func<IInstanceScope, TImplimentation> implimentationFactory)
            where TImplimentation : TInstance;

        IInstanceProvider AddTransient(Type instanceType);
        IInstanceProvider AddTransient(Type instanceType, Type implimentationType);
        IInstanceProvider AddTransient(Type instanceType, Func<IInstanceScope, object> implimentationFactory);
        IInstanceProvider AddTransient<TInstance>();
        IInstanceProvider AddTransient<TInstance, TImplimentation>()
            where TImplimentation : TInstance;
        IInstanceProvider AddTransient<TInstance>(Func<IInstanceScope, TInstance> implimentationFactory);
        IInstanceProvider AddTransient<TInstance, TImplimentation>(Func<IInstanceScope, TImplimentation> implimentationFactory)
            where TImplimentation : TInstance;
    }
}