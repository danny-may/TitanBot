using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace TitanBot.Helpers
{
    public class SynchronisedExecutor
    {
        private ConcurrentQueue<Task> ExecutionQueue { get; } = new ConcurrentQueue<Task>();
        private bool IsProcessing { get; set; }
        private object _syncLock = new object();
        public bool ProcessOnThread { get; set; }

        private void Process()
        {
            if (ProcessOnThread)
                Task.Run(() => DoWork());
            else
                DoWork();

            async void DoWork()
            {
                lock (_syncLock)
                {
                    if (IsProcessing)
                        return;
                    else
                        IsProcessing = true;
                }

                while (ExecutionQueue.TryDequeue(out var task))
                {
                    task.Start();
                    await task;
                }

                lock (_syncLock)
                {
                    IsProcessing = false;
                }
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
