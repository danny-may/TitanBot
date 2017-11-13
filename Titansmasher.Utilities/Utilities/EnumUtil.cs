using System;
using System.Collections.Generic;
using System.Linq;

namespace Titansmasher.Utilities.Utilities
{
    public class EnumUtil<TEnum>
    {
        #region Statics

        #region Public

        public static Type EnumType { get; } = typeof(TEnum);

        public static Dictionary<TEnum, string> CreateDictionary()
        {
            VerifyEnum();

            var values = Enum.GetValues(EnumType).Cast<TEnum>();
            var names = Enum.GetNames(EnumType);

            return values.Zip(names, (v, n) => (v: v, n: n))
                         .ToDictionary(e => e.v, e => e.n);
        }

        #endregion Public

        #region Private

        private static void VerifyEnum()
        {
            if (!EnumType.IsEnum)
                throw new InvalidOperationException($"{EnumType.Name} is not an enum");
        }

        #endregion Private

        #endregion Statics
    }
}