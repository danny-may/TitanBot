using System.Collections.Generic;

namespace TitanBot.Core.Services.Command.Models
{
    public interface IArgumentCollection : IReadOnlyList<IArgumentInfo>
    {
        int DenseCount { get; }
        int DensePosition { get; }
        int MaxLength { get; }
    }
}