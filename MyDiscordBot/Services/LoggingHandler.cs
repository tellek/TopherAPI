using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MyDiscordBot.Services
{
    public class LoggingHandler
    {
        public static StringBuilder DiscordLog = new StringBuilder();

        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        // DiscordSocketClient and CommandService are injected automatically from the IServiceProvider
        public LoggingHandler(DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{msg.Severity}] {msg.Source}: {msg.Exception?.ToString() ?? msg.Message}";
            DiscordLog.AppendLine(logText);                   // Write the log text to static stringBuilder
            return Task.CompletedTask;
        }
    }
}
