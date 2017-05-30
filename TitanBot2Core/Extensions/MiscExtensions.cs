using System;
using System.Linq;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.CommandService;

namespace TitanBot2.Extensions
{
    public static class MiscExtensions
    {
        private static Random _rand = new Random();
        public static Discord.Color ToDiscord(this System.Drawing.Color color)
            => new Discord.Color(color.R, color.G, color.B);

        public static System.Drawing.Color RandColor()
            => System.Drawing.Color.FromArgb(_rand.Next(255), _rand.Next(255), _rand.Next(255));

        public static async Task<string[]> GetPrefixes(this CmdContext context)
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
            return new string[]
            {
                Configuration.Instance.Prefix,
                context.Client.CurrentUser.Username,
                context.Client.CurrentUser.Mention,
                ""
            };
        }

        public static T[][] Rotate<T>(this T[][] data)
        {
            var ret = new T[data.Max(r => r.Length)][];
            for (int y = 0; y < data.Length; y++)
            {
                for (int x = 0; x < data[y].Length; x++)
                {
                    ret[x] = ret[x] ?? new T[data.Length];
                    ret[x][y] = data[y][x];
                }
            }
            return ret;
        }

        public static T[][] ForceColumns<T>(this T[][] data)
        {
            var columns = data.Max(r => r.Length);
            var ret = data.Select(r => new T[columns]).ToArray();
            for (int y = 0; y < data.Length; y++)
            {
                ret[y] = new T[columns];
                for (int x = 0; x < data[y].Length; x++)
                {
                    ret[y][x] = data[y][x];
                }
            }

            return ret;
        }

        public static Random RandInstance
            => _rand;
    }
}
