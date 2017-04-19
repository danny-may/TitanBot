using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot2.Extensions
{
    public static class MiscExtensions
    {
        public static Discord.Color ToDiscord(this System.Drawing.Color color)
            => new Discord.Color(color.R, color.G, color.B);
    }
}
