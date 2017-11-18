using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Titanbot.Command.Interfaces;
using Titanbot.Config;
using Titanbot.Extensions;
using Titanbot.Startup.Interfaces;
using Titansmasher.Services.Logging;
using Titansmasher.Services.Logging.Interfaces;
using Titansmasher.Utilities;

namespace Titanbot.LiveTesting
{
    public class TitanbotController : IStartup
    {
        #region Fields

        private DiscordSocketClient _client;
        private BotConfig _config;
        private ILoggerService _logger;
        private ICommandService _cmdService;

        private EventAwaiter _logoutEvent = new EventAwaiter();

        #endregion Fields

        #region Constructors

        public TitanbotController(DiscordSocketClient client,
                                  BotConfig config,
                                  ILoggerService logger,
                                  ICommandService commandService)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cmdService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            _client.Log += m => _logger.LogAsync(m);
            _cmdService.Install(Assembly.GetExecutingAssembly());
            _cmdService.Install(Assembly.GetAssembly(typeof(IStartup)));

            _logger.Log(LogLevel.Info, "Initialised");
        }

        #endregion Constructors

        #region Methods

        private Task OnLogout()
        {
            _client.LoggedOut -= OnLogout;

            _logoutEvent.EventFired();

            return Task.CompletedTask;
        }

        #endregion Methods

        #region IStartupService

        public Task WhileConnected => _logoutEvent.WaitEvent;

        public async Task StartAsync()
        {
            _logoutEvent.ResetEvent();

            _logger.Log(LogLevel.Info, "Starting up Client");

            await _client.LoginAsync(Discord.TokenType.Bot, _config.Token);
            await _client.StartAsync();

            _client.LoggedOut += OnLogout;
        }

        public async Task StopAsync()
        {
            _logger.Log(LogLevel.Info, "Stopping Client");

            await _client.StopAsync();
            await _client.LogoutAsync();
        }

        #region IDisposable

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        ~TitanbotController()
            => Dispose();

        #endregion IDisposable

        #endregion IStartupService
    }
}