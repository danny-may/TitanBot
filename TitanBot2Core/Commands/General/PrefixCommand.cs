﻿using Discord.Commands;
using System.Threading.Tasks;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.General
{
    public class PrefixCommand : Command
    {
        public PrefixCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            RequiredContexts = ContextType.Guild;
            DefaultPermission = 8;
            Calls.AddNew(a => GetPrefixesAsync());
            Calls.AddNew(a => SetPrefixAsync((string)a[0]))
                 .WithArgTypes(typeof(string))
                 .WithItemAsParams(0);
            Description = "Gets or sets a custom prefix that is required to use my commands";
            Usage.Add("`{0}` - Gets all the available current prefixes");
            Usage.Add("`{0} <prefix>` - Sets the custom prefix");
        }

        protected async Task GetPrefixesAsync()
        {
            var prefixes = await Context.GetPrefixes();

            if (prefixes.Length > 0)
                await ReplyAsync($"Your available prefixes are {string.Join(", ", prefixes)}", ReplyType.Success);
            else
                await ReplyAsync("You do not require prefixes in this channel", ReplyType.Success);
        }

        private async Task SetPrefixAsync(string newPrefix)
        {
            var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
            guildData.Prefix = newPrefix.ToLower();
            await Context.Database.QueryAsync(conn => conn.GuildTable.Update(guildData));
            await ReplyAsync($"Your guilds prefix has been set to `{guildData.Prefix}`", ReplyType.Success);
        }

        protected override Task<CommandCheckResponse> CheckPermissions(ulong defaultPerm)
        {
            if (Context.Arguments.Length > 0)
                return base.CheckPermissions(defaultPerm);
            else
                return base.CheckPermissions(0);
        }

        protected override Task<CommandCheckResponse> CheckContexts(ContextType contexts)
        {
            if (Context.Arguments.Length > 0)
                return base.CheckContexts(contexts);
            else
                return base.CheckContexts(ContextType.DM | ContextType.Group | ContextType.Guild);
        }
    }
}