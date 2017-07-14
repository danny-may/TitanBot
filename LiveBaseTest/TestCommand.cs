using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Commands;

namespace LiveBaseTest
{
    [Description("Command used purely for testing")]
    [RequireOwner]
    class TestCommand : Command
    {
        [Call("a")]
        async Task TestAAsync()
        {
        }
    }
}
