using System;
using System.Collections.Generic;
using System.Linq;

namespace TitanBot.Services.Dependency
{
    internal class InstanceBuilder
    {
        public bool TryConstruct(Type type, TryGetInstance objectRetrieval, out object instance)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (objectRetrieval == null)
                throw new ArgumentNullException(nameof(objectRetrieval));

            instance = null;

            Func<object> bestCtor = null;
            var bestParamsCount = -1;

            foreach (var ctor in type.GetConstructors())
            {
                if (ctor.GetParameters().Any(p => p.ParameterType == type))
                    continue;

                var paramCount = 0;
                var ctorArgs = new List<object>();

                foreach (var param in ctor.GetParameters())
                {
                    if (objectRetrieval(param.ParameterType, out var obj))
                    {
                        paramCount++;
                        ctorArgs.Add(obj);
                    }
                    else if (param.HasDefaultValue)
                        ctorArgs.Add(obj);
                    else break;
                }

                if (ctorArgs.Count != ctor.GetParameters().Length)
                    continue;

                if (paramCount > bestParamsCount)
                {
                    bestParamsCount = paramCount;
                    bestCtor = () => ctor.Invoke(ctorArgs.ToArray());
                }
            }

            if (bestCtor == null)
                return false;
            instance = bestCtor();
            return true;
        }
    }
}