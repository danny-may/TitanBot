using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Formatting;

namespace LiveBaseTest
{
    class Formatter : ValueFormatter
    {

        public Formatter()
        {
            Add<double>(Beautify);
            Add<int>(i => Beautify(i));
            Add<uint>(i => Beautify(i));
            Add<float>(i => Beautify(i));
            Add<short>(i => Beautify(i));
            Add<ushort>(i => Beautify(i));
        }

        private string Beautify(double value)
            => Math.Round(value, 3).ToString();
    }
}
