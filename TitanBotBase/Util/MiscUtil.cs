using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Util
{
    public static class MiscUtil
    {
        public static T IfDefault<T>(this T value, T otherwise)
            => IfDefault(value, () => otherwise);

        public static T IfDefault<T>(this T value, Func<T> otherwise)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
                return otherwise();
            return value;
        }
    }
}
