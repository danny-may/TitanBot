using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.Database.Models;
using TitanBot2.TypeReaders;

namespace TitanBot2.Commands.Data
{
    public class ClaimCommand : Command
    {
        public ClaimCommand(TitanbotCmdContext context, TypeReaderCollection readers) : base(context, readers)
        {
            Description = "Claims a support code. Not currently used for anything, but will be used later for API calls";
            Usage.Add("`{0} <support code>` - Claims a support code as your own.");
            Calls.AddNew(a => ClaimCodeAsync((string)a[0]))
                 .WithArgTypes(typeof(string));
            Usage.Add("`{0} <supportCode>` - Claims a single support code as your own");
            Description = "Used to tie your discord account to your ingame account. Will be used in the future for API access!";
        }

        protected async Task ClaimCodeAsync(string supportCode)
        {
            if (supportCode == null)
            {
                await ReplyAsync("You must supply a support code!", ReplyType.Error);
                return;
            }

            var supportCodeOwned = await Context.Database.Users.Find(supportCode);
            if (supportCode.Length > 7)
                await ReplyAsync("That is an invalid support code", ReplyType.Error);
            else if (supportCodeOwned != null)
                await ReplyAsync("That support code is already claimed!", ReplyType.Error);
            else
            {
                var current = await Context.Database.Users.Find(Context.User.Id);
                if (current != null && supportCode.ToLower() == current.SupportCode.ToLower())
                    await ReplyAsync($"You already have the support code `{current.SupportCode}` claimed!", ReplyType.Success);
                else
                {
                    var newUser = current ?? new User { DiscordId = Context.User.Id };
                    newUser.SupportCode = supportCode.ToLower();
                    await Context.Database.Users.Upsert(newUser);
                    await ReplyAsync($"You have claimed the support code `{supportCode}`" +
                                     (current == null ? "" : $" and given up ownership of `{current.SupportCode}`") +
                                     "\n*This currently isnt used for anything, but will be later on*", ReplyType.Success);
                }
            }
        }
    }
}
