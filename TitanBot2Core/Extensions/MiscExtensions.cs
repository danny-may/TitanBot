using Discord.Commands;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.Database;

namespace TitanBot2.Extensions
{
    public static class MiscExtensions
    {
        public static Discord.Color ToDiscord(this System.Drawing.Color color)
            => new Discord.Color(color.R, color.G, color.B);

        public static async Task<string[]> GetPrefixes(this TitanbotCmdContext context)
        {
            if (context.Guild != null)
            {
                var guildPrefix = await context.Database.Guilds.GetPrefix(context.Guild.Id);
                if (guildPrefix != Configuration.Instance.Prefix)
                    return new string[]
                    {
                        Configuration.Instance.Prefix,
                        guildPrefix,
                        context.Client.CurrentUser.Username,
                        context.Client.CurrentUser.Mention
                    };
                else
                    return new string[]
                    {
                        Configuration.Instance.Prefix,
                        context.Client.CurrentUser.Username,
                        context.Client.CurrentUser.Mention
                    };
            }
            return new string[] { };
        }
    }
}
