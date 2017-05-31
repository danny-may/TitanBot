using System.Threading.Tasks;
using TitanBot2.Services.CommandService;
using TitanBot2.Services.CommandService.Attributes;
using TitanBot2.Services.Database.Tables;

namespace TitanBot2.Commands.Data
{
    [Description("Used to tie your discord account to your ingame account. Will be used in the future for API access")]
    class ClaimCommand : Command
    {

        [Call]
        [Usage("Claims a support code as your own.")]
        async Task ClaimCodeAsync(string supportCode)
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
