using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace MyDiscordBot
{
    public class DiscordBotMain
    {
        const string Token = "NjIyODcyNzMxNjIwMzQzODMy.XX6OHQ.5c-8x6_2qB3RUPfY7p2QtV0GAaA";

        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public DiscordBotMain(IServiceProvider provider, DiscordSocketClient discord,
            CommandService commands)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            await _discord.LoginAsync(TokenType.Bot, Token);            // Login to discord
            await _discord.StartAsync();                                // Connect to the websocket

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);     // Load commands and modules into the command service
        }

    }
}
