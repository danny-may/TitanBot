using Discord;
using System;
using System.Collections.Generic;
using System.Text;

namespace TitanBot.Core.Services.Formatting
{
    public interface IEmbedable
    {
        string AsString();

        Embed AsEmbed();
    }
}