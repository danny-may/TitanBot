using System.Threading.Tasks;

namespace Titansmasher.Utilities
{
    public class EventAwaiter
    {
        public Task WaitEvent => _eventWaiter.Task;

        private TaskCompletionSource<bool> _eventWaiter = new TaskCompletionSource<bool>();

        public virtual void EventFired()
            => _eventWaiter.TrySetResult(true);

        public virtual void ResetEvent()
        {
            _eventWaiter.TrySetCanceled();
            _eventWaiter = new TaskCompletionSource<bool>();
        }
    }

    public class EventAwaiter<TArg> : EventAwaiter
    {
        public Task<TArg> WaitResult => _resultWaiter.Task;

        private TaskCompletionSource<TArg> _resultWaiter = new TaskCompletionSource<TArg>();

        public void EventFired(TArg arg)
        {
            EventFired();
            _resultWaiter.TrySetResult(arg);
        }

        public override void ResetEvent()
        {
            ResetEvent();
            _resultWaiter.TrySetCanceled();
            _resultWaiter = new TaskCompletionSource<TArg>();
        }
    }
}