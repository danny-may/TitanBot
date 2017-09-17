using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Core.Services.Command.Models;

namespace TitanBot.Services.Command.Models
{
    internal class ArgumentCollection : IArgumentCollection
    {
        #region Fields

        private readonly List<IArgumentInfo> _arguments;
        private readonly Lazy<int> _densePosition;
        private readonly Lazy<int> _denseCount;

        #endregion Fields

        #region Constructors

        public ArgumentCollection(IEnumerable<IArgumentInfo> arguments)
        {
            _arguments = arguments.ToList();
            _densePosition = new Lazy<int>(() => DenseCount == 0 ? MaxLength - 1 : _arguments.IndexOf(a => a.IsDense));
            _denseCount = new Lazy<int>(() => _arguments.Count(a => a.IsDense));
        }

        #endregion Constructors

        #region IArgumentCollection

        public IArgumentInfo this[int index] => _arguments[index];
        public int DensePosition => _densePosition.Value;
        public int DenseCount => _denseCount.Value;
        public int MaxLength => _arguments.Count;
        public int Count => _arguments.Count;

        public IEnumerator<IArgumentInfo> GetEnumerator()
            => _arguments.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)_arguments).GetEnumerator();

        #endregion IArgumentCollection
    }
}