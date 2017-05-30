﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Responses;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.CommandService.Models;

namespace TitanBot2.Commands.General
{
    [Description("Gets or sets a custom prefix that is required to use my commands")]
    [DefaultPermission(8)]
    class PrefixCommand : Command
    {
        [Call]
        [DefaultPermission(0, "Show")]
        [Usage("Gets all the available current prefixes")]
        async Task GetPrefixesAsync()
        {
            var prefixes = await Context.GetPrefixes();

            if (prefixes.Length > 0)
                await ReplyAsync($"Your available prefixes are {string.Join(", ", prefixes)}", ReplyType.Success);
            else
                await ReplyAsync("You do not require prefixes in this channel", ReplyType.Success);
        }

        [Call]
        [DefaultPermission(8, "Set")]
        [RequireContext(ContextType.Guild)]
        [Usage("Sets the custom prefix")]
        async Task SetPrefixAsync(string newPrefix)
        {
            var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
            guildData.Prefix = newPrefix.ToLower();
            await Context.Database.QueryAsync(conn => conn.GuildTable.Update(guildData));
            await ReplyAsync($"Your guilds prefix has been set to `{guildData.Prefix}`", ReplyType.Success);
        }
    }
}
