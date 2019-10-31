using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using static Contracts.AppSettings;

namespace DiscordBot
{
    public class DiscordBotMain
    {
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
            await _discord.LoginAsync(TokenType.Bot, Settings.DiscordToken);         
            await _discord.StartAsync();                             

            await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _provider);
        }

        
    }

}
