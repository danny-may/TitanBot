using System.Threading.Tasks;
using TitanBotBase.Commands;
using TT2Bot.Models;

namespace TT2Bot.Commands.Data
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

            var supportCodeOwned = await Database.FindOne<PlayerData>(p => p.PlayerCode == supportCode);
            if (supportCode.Length > 7)
                await ReplyAsync("That is an invalid support code", ReplyType.Error);
            else if (supportCodeOwned != null)
                await ReplyAsync("That support code is already claimed!", ReplyType.Error);
            else
            {
                var current = await Database.FindOne<PlayerData>(p => p.Id == Author.Id);
                if (current != null && supportCode.ToLower() == current.PlayerCode.ToLower())
                    await ReplyAsync($"You already have the support code `{current.PlayerCode}` claimed!", ReplyType.Success);
                else
                {
                    var newUser = current ?? new PlayerData { Id = Author.Id };
                    newUser.PlayerCode = supportCode.ToLower();
                    await Database.Upsert(newUser);
                    await ReplyAsync($"You have claimed the support code `{supportCode}`" +
                                     (current == null ? "" : $" and given up ownership of `{current.PlayerCode}`") +
                                     "\n*This currently isnt used for anything, but will be later on*", ReplyType.Success);
                }
            }
        }
    }
}
