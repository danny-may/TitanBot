﻿using Discord;
using System.Threading.Tasks;
using TitanBot.Commands.Models;
using TitanBot.Dependencies;
using TitanBot.Replying;

namespace TitanBot.Commands.DefaultCommands.Owner
{
    [Description(TitanBotResource.SUDOCOMMAND_HELP_DESCRIPTION)]
    [RequireOwner]
    public class SudoCommand : Command
    {
        private IDependencyFactory Factory { get; }

        public SudoCommand(IDependencyFactory factory)
        {
            Factory = factory;
        }

        [Call]
        [Usage(TitanBotResource.SUDOCOMMAND_HELP_USAGE)]
        async Task SudoAsync(IUser user, [Dense]string command)
        {
            var spoofMessage = new SudoMessage(Message);
            spoofMessage.Author = user;
            spoofMessage.Content = command;
            await ReplyAsync(TitanBotResource.SUDOCOMMAND_SUCCESS, ReplyType.Success, command, user);
            await CommandService.ParseAndExecute(spoofMessage);
        }
    }
}
