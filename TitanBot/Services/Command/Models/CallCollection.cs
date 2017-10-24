using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TitanBot.Core.Services.Command.Models;

namespace TitanBot.Services.Command.Models
{
    internal class CallCollection : ICallCollection
    {
        #region Fields

        private List<CallInfo> _calls;

        #endregion Fields

        #region Constructors

        public CallCollection(IEnumerable<CallInfo> calls)
        {
            _calls = calls.ToList();
        }

        #endregion Constructors

        #region ICallCollection

        public ICallInfo this[int index] => _calls[index];
        public int Count => _calls.Count;

        public IEnumerator<ICallInfo> GetEnumerator()
            => _calls.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)_calls).GetEnumerator();

        #endregion ICallCollection
    }
}