using System;

namespace TitanBot.Core.Services.Dependency
{
    public interface IInstanceScope
    {
        IInstanceScope ParentScope { get; }

        IInstanceScope CreateScope();

        object GetInstance(Type type);
        object GetInstance(Type type, params object[] withObjects);
        object GetRequiredInstance(Type type);
        object GetRequiredInstance(Type type, params object[] withObjects);
        //IEnumerable<object> GetInstances(Type type);
        //IEnumerable<object> GetInstances(Type type, params object[] withObjects);

        TInstance GetInstance<TInstance>();
        TInstance GetInstance<TInstance>(params object[] withObjects);
        TInstance GetRequiredInstance<TInstance>();
        TInstance GetRequiredInstance<TInstance>(params object[] withObjects);
        //IEnumerable<TInstance> GetInstances<TInstance>();
        //IEnumerable<TInstance> GetInstances<TInstance>(params object[] withObjects);
    }
}