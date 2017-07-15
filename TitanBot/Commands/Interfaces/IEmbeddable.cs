using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBot.Commands
{
    public interface IEmbedable
    {
        Embed GetEmbed();
        string GetString();
    }
}
