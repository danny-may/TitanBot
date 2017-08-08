using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static (T Result, TimeSpan Time) TimeAsyncExecution<T>(Func<ValueTask<T>> action)
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

        public static Stream ToStream(this string text)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(text);
            sw.Flush();
            ms.Position = 0;
            return ms;
        }

        public static T[][] Rotate<T>(this T[][] data)
        {
            var ret = new T[data.Max(r => r.Length)][];
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[y].Length; x++)
                {
                    ret[x] = ret[x] ?? new T[data.Length];
                    ret[x][y] = data[y][x];
                }
            }
            return ret;
        }

        public static T[][] ForceColumns<T>(this T[][] data)
        {
            var columns = data.Max(r => r.Length);
            var ret = data.Select(r => new T[columns]).ToArray();
            for (int y = 0; y < data.Length; y++)
            {
                ret[y] = new T[columns];
                for (int x = 0; x < data[y].Length; x++)
                {
                    ret[y][x] = data[y][x];
                }
            }

            return ret;
        }

        public static int IndexOf<T>(this T[] arr, Predicate<T> predicate)
        {
            for (int i = 0; i < arr.Length; i++)
                if (predicate(arr[i]))
                    return i;
            return -1;
        }
    }
}
