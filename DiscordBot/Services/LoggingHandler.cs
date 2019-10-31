using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DiscordBot.Services
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

            LogLine("It is a long established fact that a reader will be distracted by the readable content of a page when looking at its layout. The point of using Lorem Ipsum is that it has a more-or-less normal distribution of letters, as opposed to using 'Content here, content here', making it look like readable English. Many desktop publishing packages and web page editors now use Lorem Ipsum as their default model text, and a search for 'lorem ipsum' will uncover many web sites still in their infancy. Various versions have evolved over the years, sometimes by accident, sometimes on purpose (injected humour and the like).");

            LogLine(logText);                  // Write the log text to static stringBuilder
            return Task.CompletedTask;
        }

        private void LogLine(string text)
        {
            DiscordLog.AppendLine(text);
            while (DiscordLog.Length > 2000)
            {
                DiscordLog.Remove(0, Convert.ToString(DiscordLog).Split('\n').FirstOrDefault().Length + 1);
            }
        }
    }
}
