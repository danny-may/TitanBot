using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Core.Services.Command;
using TitanBot.Core.Services.Command.Models;
using TitanBot.Core.Services.Dependency;

namespace TitanBot.Services.Command
{
    public class CommandService : ICommandService
    {
        private readonly IInstanceProvider _instanceProvider;
        private readonly Dictionary<Type, List<Delegate>> _buildEvents = new Dictionary<Type, List<Delegate>>();

        public CommandService(DiscordSocketClient discord, IInstanceProvider instanceProvider)
        {
            _instanceProvider = instanceProvider;

            discord.MessageReceived += Handle;
        }

        private async Task Handle(SocketMessage message)
        {
            if (message is IUserMessage msg && !msg.Author.IsBot)
                await Handle(msg);
        }

        public IReadOnlyList<ICommandInfo> CommandHistory => throw new NotImplementedException();

        public string DefaultPrefix => throw new NotImplementedException();

        public void AddBuildEvent<T>(Func<T, Task> handler) where T : ICommand
        {
            if (!_buildEvents.ContainsKey(typeof(T)))
                _buildEvents[typeof(T)] = new List<Delegate>();
            _buildEvents[typeof(T)].Add(handler);
        }

        public Task Handle(IUserMessage message)
        {
            _instanceProvider.GetRequiredInstance<ICommandContext>(message);

            return Task.CompletedTask;
        }

        public void Install<T>() where T : class, ICommand
            => _instanceProvider.AddTransient<T>();

        public void Install(params Type[] commandTypes)
        {
            foreach (var type in commandTypes.Where(t => t.GetInterface<ICommand>() != null && !t.IsValueType))
                _instanceProvider.AddTransient(type);
        }

        public void Install(Assembly assembly)
            => Install(assembly.GetTypes());

        public void RemoveBuildEvent<T>(Func<T, Task> handler) where T : ICommand
        {
            if (!_buildEvents.ContainsKey(typeof(T)))
                _buildEvents[typeof(T)] = new List<Delegate>();
            _buildEvents[typeof(T)].Remove(handler);
        }

        public ICommandInfo[] Search(string command, out int commandLength)
        {
            throw new NotImplementedException();
        }
    }
}