using System.Linq;
using System.Linq.Expressions;

namespace System.Reflection
{
    public static class ReflectionExtensions
    {
        public static MethodInfo GetMethod<T>(Expression<Action<T>> expr)
            => ((MethodCallExpression)expr.Body)
                .Method
                .GetGenericMethodDefinition();

        public static bool IsConvertableTo<T>(this Type type)
            => type.IsConvertableTo(typeof(T));

        public static bool IsConvertableTo(this Type from, Type to)
        {
            if (from.IsSubclassOf(to))
                return true;
            if (to.IsInterface && from.GetInterfaces().Contains(to))
                return true;
            return from.GetMethods(BindingFlags.Public | BindingFlags.Static)
                       .Select(m => (MethodInfo: m, Params: m.GetParameters().ToArray()))
                       .Where(m => m.MethodInfo.Name == "op_Implicit" && m.MethodInfo.ReturnType == to && m.Params.Length == 1)
                       .Any(m => m.Params[0] != null && m.Params[0].ParameterType == from);
        }

        public static string GetMemberName<T, TValue>(Expression<Func<T, TValue>> memberAccess)
            => ((MemberExpression)memberAccess.Body).Member.Name;

        public static Type GetInterface<TInterface>(this Type type)
            => type.GetInterfaces().FirstOrDefault(i => i == typeof(TInterface));
    }
}