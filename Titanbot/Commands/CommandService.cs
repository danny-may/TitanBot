using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Titanbot.Commands.Interfaces;
using Titanbot.Commands.Models;
using Titanbot.Commands.Splitters;
using Titanbot.Config;
using Titansmasher.Services.Configuration;
using Titansmasher.Services.Configuration.Interfaces;
using Titansmasher.Services.Database.Interfaces;
using Titansmasher.Services.Database.LiteDb;
using Titansmasher.Services.Display.Interfaces;
using Titansmasher.Services.Logging.Interfaces;

namespace Titanbot.Commands
{
    public class CommandService : ICommandService
    {
        #region Fields

        private readonly DiscordSocketClient _client;
        private readonly ILoggerService _logger;
        private readonly IConfigService _config;
        private readonly IDatabaseService _database;
        private readonly IMessageSplitter _msgSplitter;
        private readonly IDisplayService _displayer;

        private readonly List<CommandInfo> _commands = new List<CommandInfo>();
        private readonly List<ServiceDescriptor> _services = new List<ServiceDescriptor>();
        private IServiceProvider _provider;

        #endregion Fields

        #region Constructors

        public CommandService(DiscordSocketClient client,
                              ILoggerService logger,
                              IDisplayService displayer,
                              IConfigService config = null,
                              IDatabaseService database = null,
                              IMessageSplitter messageSplitter = null,
                              IServiceCollection collection = null)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _displayer = displayer ?? throw new ArgumentNullException(nameof(displayer));
            _config = config ?? new ConfigService();
            _database = database ?? new LiteDbService(_config.Request<LiteDbConfig>(), _logger);
            _msgSplitter = messageSplitter ?? new MessageSplitter(_client, _config.Request<BotConfig>());

            _client.MessageReceived += HandleMessage;

            _services.Add(ServiceDescriptor.Singleton(_logger));
            _services.Add(ServiceDescriptor.Singleton(_displayer));
            _services.Add(ServiceDescriptor.Singleton(_client));
            _services.Add(ServiceDescriptor.Singleton(_config));
            _services.Add(ServiceDescriptor.Singleton(_database));
            _services.Add(ServiceDescriptor.Singleton(_msgSplitter));

            UpdateProvider();
        }

        #endregion Constructors

        #region Methods

        private void UpdateProvider()
        {
            IServiceCollection collection = new ServiceCollection();

            foreach (var desc in _services)
                collection.Add(desc);

            foreach (var command in _commands)
                collection.AddTransient(command.CommandType);

            _provider = collection.BuildServiceProvider();
        }

        private ICommandService RegisterService(ServiceDescriptor desc)
        {
            _services.Add(desc);
            UpdateProvider();
            return this;
        }

        #endregion Methods

        #region ICommandService

        public async Task HandleMessage(SocketMessage message)
        {
            if (!(message is SocketUserMessage msg) || msg.Author.IsBot)
                return;

            var context = new CommandContext(msg, _msgSplitter);

            if (context.IsCommand)
            {
                var info = Search(context.CommandName);
                if (info == null)
                    return;

                return;
            }

            return;

            //ToDo: Finish off this method
        }

        public ICommandService Install<TCommand>() where TCommand : CommandBase
            => Install(typeof(TCommand));

        public ICommandService Install(Type commandType)
            => Install(new[] { commandType });

        public ICommandService Install(Type[] commands)
        {
            var valids = commands.Where(t => t.IsSubclassOf(typeof(CommandBase)))
                                 .Where(t => !t.IsAbstract)
                                 .Where(t => !_commands.Any(c => c.CommandType == t));

            _commands.AddRange(CommandInfo.BuildFor(valids));
            return this;
        }

        public ICommandService Install(Assembly assembly)
            => Install(assembly.GetExportedTypes());

        public ICommandService Uninstall<TCommand>() where TCommand : CommandBase
            => Uninstall(typeof(TCommand));

        public ICommandService Uninstall(Type commandType)
            => Uninstall(new[] { commandType });

        public ICommandService Uninstall(Type[] commands)
        {
            _commands.RemoveAll(c => commands.Contains(c.CommandType));
            return this;
        }

        public ICommandService Uninstall(Assembly assembly)
            => Uninstall(assembly.GetExportedTypes());

        public ICommandService RegisterService<TService>()
            where TService : class
            => RegisterService(ServiceDescriptor.Singleton<TService, TService>());

        public ICommandService RegisterService<TService>(TService service)
            where TService : class
            => RegisterService(ServiceDescriptor.Singleton(service));

        public ICommandService RegisterService<TService, TImplimentation>()
            where TImplimentation : class, TService
            where TService : class
            => RegisterService(ServiceDescriptor.Singleton<TService, TImplimentation>());

        public ICommandService RegisterService<TService, TImplimentation>(TImplimentation service)
            where TImplimentation : class, TService
            where TService : class
            => RegisterService(ServiceDescriptor.Singleton<TService>(service));

        public CommandInfo Search(string commandName)
        {
            var byName = _commands.FirstOrDefault(
                                       c => c.Name
                                             .Equals(commandName,
                                                     StringComparison.InvariantCultureIgnoreCase));
            if (byName != null)
                return byName;
            return _commands.FirstOrDefault(
                                 c => c.Alias
                                       .Any(a =>
                                            a.Equals(commandName,
                                                     StringComparison.InvariantCultureIgnoreCase)));
        }

        #endregion ICommandService
    }
}