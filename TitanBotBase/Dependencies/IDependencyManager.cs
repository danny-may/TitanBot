using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Dependencies
{
    public interface IDependencyManager : IDisposable
    {
        void Add<T>(T value);
        void Add(Type type, object value);
        bool TryGet<T>(out T result);
        bool TryGet(Type type, out object result);
        bool TryConstruct<T>(out T obj);
        bool TryConstruct<T>(out T obj, params Type[] pattern);
    }
}
