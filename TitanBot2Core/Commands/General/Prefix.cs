using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using TitanBot2.Common;
using TitanBot2.Extensions;
using TitanBot2.Services.CommandService;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.General
{
    public class Prefix : Command
    {
        public Prefix(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            RequiredContexts = ContextType.Guild;
            DefaultPermission = 8;
            SubCommands.Add(".+", SetPrefixAsync);
            Description = "Gets or sets a custom prefix that is required to use my commands";
            Usage = new string[]
            {
                "`{0}` - Gets all the available current prefixes",
                "`{0} <prefix>` - Sets the custom prefix"
            };
        }

        protected override async Task RunAsync()
        {
            var prefixes = await Context.GetPrefixes();

            if (prefixes.Length > 0)
                await ReplyAsync($"{Res.Str.InfoText} Your available prefixes are {string.Join(", ", prefixes)}");
            else
                await ReplyAsync($"{Res.Str.InfoText} You do not require prefixes in this channel");
        }

        private async Task SetPrefixAsync()
        {
            var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
            guildData.Prefix = Context.Arguments.First().ToLower();
            await Context.Database.QueryAsync(conn => conn.GuildTable.Update(guildData));
            await ReplyAsync($"{Res.Str.SuccessText} Your guilds prefix has been set to `{guildData.Prefix}`");
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
