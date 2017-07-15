using System;
using System.Collections.Generic;
using System.IO;
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

        public static T InlineAction<T>(T obj, Action<T> action)
        {
            action(obj);
            return obj;
        }


        public static void CheckNull<T>(this T? value, Action<T> hasValue)
            where T : struct => CheckNull(value, hasValue, () => { });
        public static void CheckNull<T>(this T? value, Action noValue)
            where T : struct => CheckNull<T>(value, t => { }, noValue);
        public static void CheckNull<T>(this T? value, Action<T> hasValue, Action noValue) 
            where T : struct
        {
            if (value.HasValue)
                hasValue(value.Value);
            else
                noValue();
        }
        public static void CheckNull<T>(this T value, Action<T> hasValue)
            where T : class => CheckNull(value, hasValue, () => { });
        public static void CheckNull<T>(this T value, Action noValue)
            where T : class => CheckNull(value, t => { }, noValue);
        public static void CheckNull<T>(this T value, Action<T> hasValue, Action noValue)
            where T : class
        {
            if (value != null)
                hasValue(value);
            else
                noValue();
        }

        public static Stream ToStream(this string text)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(text);
            sw.Flush();
            ms.Position = 0;
            return ms;
        }
    }
}
