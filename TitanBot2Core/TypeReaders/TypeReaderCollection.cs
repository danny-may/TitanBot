using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Services.CommandService;

namespace TitanBot2.TypeReaders
{
    public class TypeReaderCollection
    {
        private Dictionary<Type, Type> _typeReaders = new Dictionary<Type, Type>();

        private object _lock = new object();

        public Task Install(Assembly assembly)
        {
            return Task.Run(() =>
            {
                lock (_lock)
                {
                    var readerQuery = assembly.GetTypes()
                                            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(TypeReader)));
                    foreach (var type in readerQuery)
                    {
                        var reader = Activator.CreateInstance(type) as TypeReader;
                        _typeReaders.Add(reader.Target, type);
                    }
                }
            });
        }

        public async Task<TypeReaderResponse<T>> Read<T>(TitanbotCmdContext context, string text)
        {
            if (_typeReaders.ContainsKey(typeof(T)))
                return await (Activator.CreateInstance(_typeReaders[typeof(T)]) as TypeReader<T>)?.Read(context, text) ?? null;

            return TypeReaderResponse<T>.FromError($"No reader found for type `{typeof(T).FullName}`");
        }
    }
}
