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
    [RequireContext(ContextType.Guild)]
    class PrefixCommand : Command
    {
        [Call]
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
        [Usage("Sets the custom prefix")]
        async Task SetPrefixAsync(string newPrefix)
        {
            var guildData = await Context.Database.Guilds.GetGuild(Context.Guild.Id);
            guildData.Prefix = newPrefix.ToLower();
            await Context.Database.QueryAsync(conn => conn.GuildTable.Update(guildData));
            await ReplyAsync($"Your guilds prefix has been set to `{guildData.Prefix}`", ReplyType.Success);
        }

        protected override Task<CallCheckResponse> CheckPermissions(ulong defaultPerm, string permKey)
        {
            if (Context.Arguments.Length > 0)
                return base.CheckPermissions(defaultPerm, permKey);
            else
                return base.CheckPermissions(0, permKey);
        }

        protected override Task<CallCheckResponse> CheckContexts(ContextType contexts)
        {
            if (Context.Arguments.Length > 0)
                return base.CheckContexts(contexts);
            else
                return base.CheckContexts(ContextType.DM | ContextType.Group | ContextType.Guild);
        }
    }
}
