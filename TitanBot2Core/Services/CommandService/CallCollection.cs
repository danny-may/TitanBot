using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Services.CommandService
{
    public class CallCollection : IEnumerable<CommandCall>
    {
        private List<CommandCall> _calls;
        public IEnumerator<CommandCall> GetEnumerator()
            => _calls.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _calls.GetEnumerator();

        public CallCollection()
        {
            _calls = new List<CommandCall>();
        }

        public CommandCall AddNew(Func<object[], Task> call)
        {
            var result = new CommandCall(call);
            _calls.Add(result);
            return result;
        }
    }
}
