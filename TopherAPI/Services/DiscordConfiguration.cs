using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot;
using DiscordBot.Services;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TopherAPI.Services
{
    public static class DiscordConfiguration
    {
        public static void ConfigureDiscord(this IServiceCollection services)
        {
            services.AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
            {                                       
                LogLevel = LogSeverity.Verbose,     
                MessageCacheSize = 1000             
            }))
            .AddSingleton(new CommandService(new CommandServiceConfig
            {                                       
                LogLevel = LogSeverity.Verbose,     
                DefaultRunMode = RunMode.Async,     
            }))
            .AddSingleton<CommandHandler>()         
            .AddSingleton<DiscordBotMain>()         
            .AddSingleton<LoggingHandler>()         
            .AddSingleton<Random>();                
        }
    }
}
