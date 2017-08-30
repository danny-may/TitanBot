using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TitanBot.Core.Models;
using TitanBot.Core.Services;

namespace TitanBot.Services
{
    public class StartupService : IStartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IConfiguration _config;

        public StartupService(DiscordSocketClient discord, IConfiguration config)
        {
            _discord = discord;
            _config = config;
        }

        public Task WhileRunning => throw new NotImplementedException();

        public async Task StartAsync()
        {
            string token = _config.Token;
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token has not yet been set in the config file.", nameof(_config.Token));

            await _discord.LoginAsync(TokenType.Bot, token);
            await _discord.StartAsync();
        }

        public Task StopAsync()
        {
            throw new NotImplementedException();
        }
    }
}