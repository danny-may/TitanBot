using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Titanbot.Commands.Interfaces;
using Titanbot.Commands.Models;
using Titanbot.Permissions.Interfaces;

namespace Titanbot.Commands
{
    public class CommandService : ICommandService
    {
        #region Fields

        private readonly DiscordSocketClient _client;
        private readonly CommandConfig _config;
        private readonly IMessageSplitter _msgSplitter;
        private readonly IPermissionManager _permManager;

        private readonly List<CommandInfo> _commands = new List<CommandInfo>();
        private readonly IServiceCollection _internalServices = new ServiceCollection();
        private readonly ICollection<ServiceDescriptor> _externalServices;
        private IServiceProvider _provider;

        #endregion Fields

        #region Constructors

        public CommandService(DiscordSocketClient client,
                              IMessageSplitter splitter,
                              IPermissionManager permManager,
                              IOptions<CommandConfig> config,
                              IServiceCollection services = null)
            : this(client, splitter, permManager, config.Value, services) { }

        public CommandService(DiscordSocketClient client,
                              IMessageSplitter splitter,
                              IPermissionManager permManager,
                              CommandConfig config,
                              IServiceCollection services = null)
        {
            _config = config;
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _msgSplitter = splitter ?? throw new ArgumentNullException(nameof(splitter));
            _permManager = permManager ?? throw new ArgumentNullException(nameof(permManager));
            _externalServices = services;

            _client.MessageReceived += HandleMessage;

            UpdateProvider();
        }

        #endregion Constructors

        #region Methods

        private void UpdateProvider()
        {
            IServiceCollection collection = new ServiceCollection();

            var allDescriptors = _internalServices.Concat(_externalServices);

            foreach (var descriptor in allDescriptors)
                collection.Add(descriptor);

            foreach (var command in _commands)
                collection.AddTransient(command.CommandType);

            _provider = collection.BuildServiceProvider();
        }

        private ICommandService RegisterService(ServiceDescriptor desc)
        {
            _internalServices.Add(desc);
            UpdateProvider();
            return this;
        }

        #endregion Methods

        #region ICommandService

        public async Task HandleMessage(SocketMessage message)
        {
            if (!(message is SocketUserMessage msg) || msg.Author.IsBot)
                return;

            var context = new MessageContext(msg, _msgSplitter);

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