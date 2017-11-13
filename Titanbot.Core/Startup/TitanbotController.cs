using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Titanbot.Core.Config;
using Titanbot.Core.Extensions;
using Titanbot.Core.Startup.Interfaces;
using Titansmasher.Services.Configuration.Interfaces;
using Titansmasher.Services.Logging;
using Titansmasher.Services.Logging.Interfaces;
using Titansmasher.Utilities;

namespace Titanbot.Core.Startup
{
    public class TitanbotController : ITitanbotController
    {
        #region Fields

        private DiscordSocketClient _socketClient;
        private DiscordShardedClient _shardedClient;

        private IConfigService _configService;
        private DiscordConfig _config;
        private IAreaLogger _areaLogger;
        private ILoggerService _logger;

        private EventAwaiter _logoutEvent = new EventAwaiter();

        #endregion Fields

        #region Constructors

        public TitanbotController(DiscordSocketClient client, IConfigService config, ILoggerService logger)
        {
            _socketClient = client ?? throw new ArgumentNullException(nameof(client));
            _configService = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _areaLogger = logger.CreateAreaLogger<TitanbotController>();
            _config = _configService.Request<DiscordConfig>();

            _socketClient.Log += m => { _logger.Log("Discord", m); return Task.CompletedTask; };

            _areaLogger.Log(LogLevel.Info, "Initialised Socket Client");
        }

        public TitanbotController(DiscordShardedClient client, IConfigService config, ILoggerService logger)
        {
            _shardedClient = client ?? throw new ArgumentNullException(nameof(client));
            _configService = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _areaLogger = logger.CreateAreaLogger<TitanbotController>();
            _config = _configService.Request<DiscordConfig>();

            _shardedClient.Log += m => { _logger.Log("Discord", m); return Task.CompletedTask; };

            _areaLogger.Log(LogLevel.Info, "Initialised Sharded Client");
        }

        #endregion Constructors

        #region Methods

        private async Task StartSocketClient()
        {
            await _socketClient.LoginAsync(Discord.TokenType.Bot, _config.Token);
            await _socketClient.StartAsync();

            _socketClient.LoggedOut += OnLogout;
        }

        private Task StartShardedClient()
        {
            throw new NotImplementedException();
        }

        private async Task StopSocketClient()
        {
            await _socketClient.StopAsync();
            await _socketClient.LogoutAsync();
        }

        private Task StopShardedClient()
        {
            throw new NotImplementedException();
        }

        private Task OnLogout()
        {
            if (_socketClient != null)
                _socketClient.LoggedOut -= OnLogout;
            else if (_shardedClient != null)
                _shardedClient.LoggedOut -= OnLogout;

            _logoutEvent.EventFired();

            return Task.CompletedTask;
        }

        #endregion Methods

        #region IStartupService

        public Task WhileConnected => _logoutEvent.WaitEvent;

        public async Task StartAsync()
        {
            _logoutEvent.ResetEvent();

            _areaLogger.Log(LogLevel.Info, "Starting up Client");
            if (_socketClient != null)
                await StartSocketClient();
            else if (_shardedClient != null)
                await StartShardedClient();
        }

        public async Task StopAsync()
        {
            _areaLogger.Log(LogLevel.Info, "Stopping Client");

            if (_socketClient != null)
                await StopSocketClient();
            else if (_shardedClient != null)
                await StopShardedClient();
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