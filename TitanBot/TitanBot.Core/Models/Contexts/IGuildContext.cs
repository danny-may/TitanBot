using Discord;
using System;
using System.Collections.Generic;
using System.Text;
using TitanBot.Core.Services.Formatting;

namespace TitanBot.Core.Models.Contexts
{
    public interface IGuildContext : ITranslationContext
    {
        IGuild Guild { get; }
    }
}