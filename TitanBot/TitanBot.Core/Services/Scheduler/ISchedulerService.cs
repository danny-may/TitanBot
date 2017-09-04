using System;
using System.Collections.Generic;

namespace TitanBot.Core.Services.Scheduler
{
    public interface ISchedulerService
    {
        bool Enabled { get; set; }
        ulong Queue(ISchedulerRecord record);
        ulong[] Queue(IEnumerable<ISchedulerRecord> records);
        ulong Queue<TCallback>(DateTime from, TimeSpan? interval = null, DateTime? to = null, string payload = null) where TCallback : ISchedulerCallback;
        ISchedulerRecord[] Cancel<TCallback>(Func<ISchedulerRecord, bool> predicate) where TCallback : ISchedulerCallback;
        ISchedulerRecord[] Cancel(Func<ISchedulerRecord, bool> predicate);
        bool Cancel(ISchedulerRecord record);
        Dictionary<ISchedulerRecord, bool> Cancel(IEnumerable<ISchedulerRecord> records);
        ISchedulerRecord Cancel(ulong id);
        ISchedulerRecord[] Cancel(IEnumerable<ulong> ids);
        ISchedulerRecord[] GetActive();
        ISchedulerRecord[] GetActive<TCallback>() where TCallback : ISchedulerCallback;
        ISchedulerRecord[] GetRecent<TCallback>(Func<ISchedulerRecord, bool> predicate, int limit = 1) where TCallback : ISchedulerCallback;
        void Register<TCallback>(TCallback callback) where TCallback : ISchedulerCallback;
    }
}
