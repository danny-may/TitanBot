using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using TitanBot.Core.Models;
using TitanBot.Core.Services;
using TitanBot.Core.Services.Logging;

namespace TitanBot.Services
{
    public class StartupService : IStartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;

        public StartupService(DiscordSocketClient discord, ILoggerService logger, IConfiguration config)
        {
            _discord = discord;
            _config = config;
            _logger = logger;

            _discord.Log += _logger.LogAsync;
        }

        public Task WhileRunning => Task.Delay(-1);

        public async Task StartAsync()
        {
            if (_discord.LoginState != LoginState.LoggedOut)
                throw new InvalidOperationException("Unable to log in while already logged in.");

            string token = _config.Token;
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token has not yet been set in the config file.", nameof(_config.Token));

            await _discord.LoginAsync(TokenType.Bot, token);
            await _discord.StartAsync();
        }

        public async Task StopAsync()
        {
            if (_discord.LoginState != LoginState.LoggedIn)
                throw new InvalidOperationException("Unable to log out while logged out");

            await _discord.StopAsync();
            await _discord.LogoutAsync();
        }
    }
}