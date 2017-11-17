using System;

namespace Titanbot.Core.Command
{
    public abstract class Command : IDisposable
    {
        public virtual void Dispose()
        {
        }
    }
}