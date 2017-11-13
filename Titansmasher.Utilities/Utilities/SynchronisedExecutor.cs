using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Titansmasher.Utilities
{
    public class SynchronisedExecutor
    {
        private ConcurrentQueue<Task> ExecutionQueue { get; } = new ConcurrentQueue<Task>();
        private bool IsProcessing { get; set; }
        private object _syncLock = new object();
        public bool ProcessOnThread { get; set; }

        private void Process()
        {
            lock (_syncLock)
            {
                if (IsProcessing)
                    return;
                IsProcessing = true;
            }

            if (ProcessOnThread)
                Task.Run(RunQueue).Wait();
            else
                RunQueue().Wait();

            lock (_syncLock)
            {
                IsProcessing = false;
            }
        }

        private async Task RunQueue()
        {
            while (ExecutionQueue.TryDequeue(out var task))
            {
                task.Start();
                await task;
            }
        }

        private async Task Run(Task task)
        {
            ExecutionQueue.Enqueue(task);
            Process();
            await task;
        }

        private async Task<T> Run<T>(Task<T> task)
        {
            ExecutionQueue.Enqueue(task);
            Process();
            return await task;
        }

        public Task Run(Action action)
            => Run(new Task(action));

        public Task<T> Run<T>(Func<T> action)
            => Run(new Task<T>(action));
    }
}