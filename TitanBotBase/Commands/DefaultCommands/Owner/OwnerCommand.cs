using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TitanBotBase.Commands.DefaultCommands.Owner
{
    [Description("Adds a new user to the owner list")]
    [RequireOwner]
    class OwnerCommand : Command
    {
        [Call("Add")]
        async Task AddOwnerAsync(IUser[] users)
        {
            var newOwners = GlobalSettings.Owners.ToList();
            newOwners.AddRange(users.Select(u => u.Id));
            GlobalSettings.Owners = newOwners.Distinct().ToArray();
            await ReplyAsync($"I have given owner status to {string.Join(", ", users.Select(u => u.Mention))}!", ReplyType.Success);
        }

        [Call("Remove")]
        async Task RemoveOwnerAsync(IUser[] users)
        {
            var newOwners = GlobalSettings.Owners.ToList();
            newOwners.RemoveAll(o => users.Any(u => u.Id == o));
            GlobalSettings.Owners = newOwners.Distinct().ToArray();
            await ReplyAsync($"I have removed owner status from {string.Join(", ", users.Select(u => u.Mention))}.", ReplyType.Success);
        }
    }
}
