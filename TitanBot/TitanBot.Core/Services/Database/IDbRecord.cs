using System;
using System.Collections.Generic;
using System.Text;

namespace TitanBot.Core.Services.Database
{
    public interface IDbRecord<TId>
    {
        TId Id { get; set; }
    }

    public interface IDbRecord : IDbRecord<ulong>
    {
    }
}