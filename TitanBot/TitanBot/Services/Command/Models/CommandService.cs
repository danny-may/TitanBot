using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services.Command;
using TitanBot.Core.Services.Command.Models;

namespace TitanBot.Services.Command.Models
{
    public class CommandService : ICommandService
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandService(DiscordSocketClient discord, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            discord.MessageReceived += Handle;
        }

        private async Task Handle(SocketMessage message)
        {
            if (message is IUserMessage msg && !msg.Author.IsBot)
                await Handle(msg);
        }

        public IReadOnlyList<ICommandInfo> CommandHistory => throw new NotImplementedException();

        public string DefaultPrefix => throw new NotImplementedException();

        public void AddBuildEvent<T>(Action<T> handler) where T : ICommand
        {
            throw new NotImplementedException();
        }

        public Task Handle(IUserMessage message)
        {
            var collection = _serviceProvider.GetRequiredService<IServiceCollection>();

            collection.AddSingleton(message);

            var provider = collection.BuildServiceProvider();

            provider.GetRequiredService<ICommandContext>();

            return Task.CompletedTask;
        }

        public void Install<T>() where T : ICommand
        {
            throw new NotImplementedException();
        }

        public void Install(Type commandType)
        {
            throw new NotImplementedException();
        }

        public void Install(Type[] commandTypes)
        {
            throw new NotImplementedException();
        }

        public void Install(Assembly assembly)
        {
            throw new NotImplementedException();
        }

        public void RemoveBuildEvent<T>(Action<T> handler) where T : ICommand
        {
            throw new NotImplementedException();
        }

        public ICommandInfo[] Search(string command, out int commandLength)
        {
            throw new NotImplementedException();
        }
    }
}