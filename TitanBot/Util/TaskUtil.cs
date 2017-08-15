using System.Collections.Generic;
using System.Linq;

namespace System.Threading.Tasks
{
    public static class TaskUtil
    {
        public static void DontWait(this Task task) { }
        public static void DontWait<T>(this ValueTask<T> task) { }
        public static void Wait<T>(this ValueTask<T> task)
            => task.GetAwaiter().GetResult();
    }

    public static class ValueTask
    {
        public static ValueTask<TResult> FromResult<TResult>(TResult result)
            => new ValueTask<TResult>(result);

        public static ValueTask<TResult[]> WhenAll<TResult>(IEnumerable<ValueTask<TResult>> tasks)
            => WhenAll(tasks.ToArray());
        public static async ValueTask<TResult[]> WhenAll<TResult>(params ValueTask<TResult>[] tasks)
        {
            var results = new TResult[tasks.Length];
            for (int i = 0; i < tasks.Length; i++)
                results[i] = await tasks[i];
            return results;
        }
    }
}
