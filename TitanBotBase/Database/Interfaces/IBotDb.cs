using System;
using System.Threading.Tasks;

namespace TitanBotBase.Database
{
    public interface IBotDb : IDisposable
    {
        int TotalCalls { get; }

        Task QueryAsync(Action<IBotDbTransaction> query);
        Task<T> QueryAsync<T>(Func<IBotDbTransaction, T> query);
    }
}
