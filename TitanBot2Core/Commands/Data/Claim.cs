using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TitanBot2.Common;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.Database.Models;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Data
{
    public class Claim : Command
    {
        public Claim(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Description = "Claims a support code. Not currently used for anything, but will be used later for API calls";
            Usage = new string[]
            {
                "`{0} <support code>` - Claims a support code as your own."
            };
        }

        protected override async Task RunAsync()
        {
            var supportCode = Context.Arguments.FirstOrDefault();

            if (supportCode == null)
            {
                await ReplyAsync($"{Res.Str.ErrorText} You must supply a support code!");
                return;
            }

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
