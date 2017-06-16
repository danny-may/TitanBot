using System.Threading.Tasks;
using TitanBotBase.Commands;

namespace TT2Bot.Commands
{
    [Description("Put the Description of your command here")]
    [RequireContext(ContextType.Guild)] //Means the command can only be used on guilds, not in DM or on group chats
    [Notes("Add any notes for the command here")]
    [Name("You can give your command a custom name here")]
    [Alias("You", "can", "give", "multiple", "aliases", "here")]
    [DoNotInstall] // Dont include this, this is just so this remains a template
    class CommandTemplate : Command
    {
        [Call]
        [Usage("Provide text on how this call is used here")]
        async Task SampleCallAsync()
        {
            //This is a sample command call that can be used with no arguments
            await ReplyAsync("Template works!", ReplyType.Success);
        }

        [Call("Sample")] // This tells it to only respond if the first argument given to the command is "Sample"
        [Usage("Provide text on how this call is used here")]
        async Task SampleCallAsync(int arg1, string arg2, [Dense]string longarg3, bool arg4)
        {
            await ReplyAsync($"You gave the arguments:\n" + 
                             $"arg1: {arg1}\n" +
                             $"arg2: {arg2}\n" +
                             $"arg3: {longarg3}\n" +
                             $"arg4: {arg4}", ReplyType.Success);
        }
    }
}
