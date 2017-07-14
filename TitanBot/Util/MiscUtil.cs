using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace TitanBot.Util
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

        public static (T Result, TimeSpan Time) TimeExecution<T>(Func<T> action)
        {
            var start = DateTime.Now;
            var result = action();
            return (result, DateTime.Now - start);
        }

        public static (T Result, TimeSpan Time) TimeAsyncExecution<T>(Func<Task<T>> action)
        {
            var start = DateTime.Now;
            var result = action().Result;
            return (result, DateTime.Now - start);
        }

        public static TimeSpan TimeExecution(Action action)
            => TimeExecution<object>(() => { action(); return null; }).Time;
        public static TimeSpan TimeAsyncExecution(Func<Task> action)
            => TimeAsyncExecution<object>(async () => { await action(); return null; }).Time;

        public static MethodInfo GetMethod<T>(Expression<Action<T>> expr)
        {
            return ((MethodCallExpression)expr.Body)
                .Method
                .GetGenericMethodDefinition();
        }
    }
}
