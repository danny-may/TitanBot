using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TitanBot.Core.Models;
using TitanBot.Core.Models.Contexts;
using TitanBot.Core.Services.Command;
using TitanBot.Core.Services.Command.Models;
using TitanBot.Core.Services.Dependency;
using TitanBot.Services.Command.Models;
using TitanBot.Utility;

namespace TitanBot.Services.Command
{
    public class CommandService : ICommandService
    {
        #region Fields

        private readonly IInstanceScope _instanceScope;
        private readonly ConcurrentDictionary<Type, List<Delegate>> _buildEvents = new ConcurrentDictionary<Type, List<Delegate>>();
        private readonly List<ICommandInfo> _installedCommands = new List<ICommandInfo>();
        private readonly List<ICommandInfo> _commandHistory = new List<ICommandInfo>();
        private readonly ProcessingQueue _commandQueue = new ProcessingQueue();
        private readonly ProcessingQueue _messageQueue = new ProcessingQueue { ProcessOnThread = true };
        private readonly IConfiguration _config;

        #endregion Fields

        #region Constructors

        public CommandService(DiscordSocketClient discord, IInstanceScope instanceScope, IConfiguration config)
        {
            _instanceScope = instanceScope;
            _config = config;

            discord.MessageReceived += m => m is SocketUserMessage msg ? Handle(msg) : Task.CompletedTask;
        }

        #endregion Constructors

        #region Methods

        private void ParseAndExecute(SocketUserMessage message)
        {
            var context = _instanceScope.GetRequiredInstance<ICommandContext>(message as IUserMessage);
        }

        #endregion Methods

        #region ICommandService

        public IReadOnlyList<ICommandInfo> CommandHistory => _commandHistory.AsReadOnly();
        public IReadOnlyList<ICommandInfo> InstalledCommands => _installedCommands.AsReadOnly();
        public string DefaultPrefix => _config.Prefix;

        public void AddBuildEvent<T>(Func<T, Task> handler) where T : ICommand
            => _buildEvents.GetOrAdd(typeof(T), k => new List<Delegate>())
                           .Add(handler);

        public Task Handle(SocketUserMessage message)
            => _messageQueue.Run(() => ParseAndExecute(message));

        public void Install(params Type[] commandTypes)
            => _installedCommands.AddRange(
                CommandInfo.Build(
                    commandTypes.Where(t => !t.IsValueType &&
                                            t.GetInterface<ICommand>() != null)
                    )
                );

        public void Install(Assembly assembly)
            => Install(assembly.GetTypes());

        public void Install<T>() where T : class, ICommand
            => Install(typeof(T));

        public void RemoveBuildEvent<T>(Func<T, Task> handler) where T : ICommand
            => _buildEvents.GetOrAdd(typeof(T), k => new List<Delegate>())
                           .Remove(handler);

        public ICommandInfo[] Search(string command, out int commandLength)
        {
            //_installedCommands.Where(c => string.Equals(c.Name, command, StringComparison.OrdinalIgnoreCase) ||
            //                              c.Alias.Any(a => string.Equals(a, command, StringComparison.OrdinalIgnoreCase)))
            //                  .ToArray();

            throw new NotImplementedException();
        }

        #endregion ICommandService
    }
}