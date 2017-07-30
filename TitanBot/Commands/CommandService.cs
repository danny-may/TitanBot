using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TitanBot.Dependencies;
using TitanBot.Logging;
using TitanBot.Settings;
using TitanBot.Storage;
using TitanBot.TypeReaders;

namespace TitanBot.Commands
{
    public class CommandService : ICommandService
    {
        public IReadOnlyList<CommandInfo> CommandList { get; }
        private List<CommandInfo> Command { get; } = new List<CommandInfo>();
        private IDependencyFactory DependencyFactory { get; }
        private ILogger Logger { get; }
        private ITypeReaderCollection Readers { get; }
        private IDatabase Database { get; }
        private ISettingManager SettingsManager { get; }
        private BotClient Client { get; }
        private DiscordSocketClient DiscordClient { get; }
        private IPermissionManager PermissionManager { get; }
        private Dictionary<Type, List<(MethodInfo Method, object Parent)>> CommandBuildActions { get; } = new Dictionary<Type, List<(MethodInfo, object)>>();

        public CommandService(IDependencyFactory factory,
                              ILogger logger,
                              DiscordSocketClient discordClient,
                              BotClient botClient,
                              ITypeReaderCollection readers,
                              IDatabase database,
                              ISettingManager settings,
                              IPermissionManager permissionManager)
        {
            DependencyFactory = factory;
            Logger = logger;
            DiscordClient = discordClient;
            Client = botClient;
            Readers = readers;
            Database = database;
            CommandList = Command.AsReadOnly();
            SettingsManager = settings;
            PermissionManager = permissionManager;
        }

        public void Install(Assembly assembly)
        {
            var commandTypes = assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(Command)) &&
                                                              !t.IsAbstract);
            Install(commandTypes.ToArray());
        }

        public void Install(params Type[] commandTypes)
        {
            var built = CommandInfo.BuildFrom(commandTypes).ToList();
            Command.AddRange(built);
            var builtCount = built.Count();
            var callCount = built.SelectMany(c => c.Calls).Count();
            var argCombCount = built.SelectMany(c => c.Calls).SelectMany(c => c.ArgumentPermatations).Count();
            Logger.Log(Logging.LogSeverity.Info, LogType.Command, $"Loaded {builtCount} command(s) | {callCount} call(s) | {argCombCount} argument combination(s)", "CommandService");
        }

        public CommandInfo? Search(string command, out int commandLength)
        {
            foreach (var cmd in CommandList)
            {
                var match = Regex.Match(command, $@"^{cmd.Name}(\b| +)", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    commandLength = match.Length;
                    return cmd;
                }
                foreach (var alias in cmd.Alias)
                {
                    match = Regex.Match(command, $@"^{alias}(\b| +)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        commandLength = match.Length;
                        return cmd;
                    }
                }
            }
            commandLength = 0;
            return null;
        }

        public Task ParseAndExecute(IUserMessage message)
        {
            var context = DependencyFactory.WithInstance(message)
                                           .Construct<ICommandContext>();
            context.CheckCommand(this, SettingsManager.GetContext(SettingsManager.Global).Get<GeneralGlobalSetting>().DefaultPrefix);
            var executor = DependencyFactory.WithInstance(context)
                                            .WithInstance(this)
                                            .Construct<CommandExecutor>();
            return executor.Run();
        }

        public void AddBuildEvent<T>(Action<T> handler) where T : Command
        {
            if (!CommandBuildActions.ContainsKey(typeof(T)))
                CommandBuildActions[typeof(T)] = new List<(MethodInfo, object)>();

            CommandBuildActions[typeof(T)].Add((handler.Method, handler.Target));
        }

        public List<Action<Command>> GetBuildEvents(Type commandType)
            => CommandBuildActions.Where(k => commandType.IsAssignableFrom(k.Key))
                                  .SelectMany(k => k.Value)
                                  .Select(h => BuildCommandAction(h.Method, h.Parent))
                                  .ToList();

        private Action<Command> BuildCommandAction(MethodInfo method, object target)
            => c => method.Invoke(target, new object[] { c });

        public List<Action<T>> GetBuildEvents<T>() where T : Command
            => GetBuildEvents(typeof(T)).Select(e => (Action<T>)e).ToList();
    }
}
