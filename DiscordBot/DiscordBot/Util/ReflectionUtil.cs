using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBot.Util
{
    public static class ReflectionUtil
    {
        public static T Default<T>()
            => default(T);
        public static object Default(Type type)
            => typeof(ReflectionUtil).GetMethod("Default", new Type[0])
                                     .MakeGenericMethod(type)
                                     .Invoke(null, null);
    }
}
