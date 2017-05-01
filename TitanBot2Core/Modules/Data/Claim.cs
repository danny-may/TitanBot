using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Preconditions;
using TitanBot2.Services.Database.Models;

namespace TitanBot2.Modules.Data
{
    public partial class DataModule
    {
        [Group("Claim")]
        [Summary("Claims an ingame support code to link it to your discord account")]
        [RequireCustomPermission(0)]
        public class Claim : TitanBotModule
        {
            [Command(RunMode = RunMode.Async)]
            public async Task ClaimAsync(string supportCode)
            {
                var supportCodeOwned = await Context.Database.Users.Find(supportCode);
                if (supportCode.Length > 7)
                    await ReplyAsync($"{Res.Str.ErrorText} That is an invalid support code");
                else if (supportCodeOwned != null)
                    await ReplyAsync($"{Res.Str.ErrorText} That support code is already claimed!");
                else
                {
                    var current = await Context.Database.Users.Find(Context.User.Id);
                    if (current != null && supportCode.ToLower() == current.SupportCode.ToLower())
                        await ReplyAsync($"{Res.Str.SuccessText} You already have the support code `{current.SupportCode}` claimed!");
                    else
                    {
                        var newUser = current ?? new User { DiscordId = Context.User.Id };
                        newUser.SupportCode = supportCode.ToLower();
                        await Context.Database.Users.Upsert(newUser);
                        await ReplyAsync($"{Res.Str.SuccessText} You have claimed the support code `{supportCode}`" +
                                         (current == null ? "" : $" and given up ownership of `{current.SupportCode}`") + 
                                         "\n*This currently isnt used for anything, but will be later on*");
                    }
                }
            }
        }
    }
}
