using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using static Contracts.AppSettings;
using System.Threading.Tasks;

namespace DiscordBot
{
    public class DiscordCommands
    {
        private readonly DiscordSocketClient _discord;

        public DiscordCommands(DiscordSocketClient discord)
        {
            _discord = discord;
        }

        public void Say(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            var chnl = _discord.GetChannel(Settings.DiscordChannel) as IMessageChannel;
            chnl.SendMessageAsync(message).GetAwaiter().GetResult();
            Console.WriteLine(message);
        }

        public async Task SayAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            var chnl = _discord.GetChannel(Settings.DiscordChannel) as IMessageChannel;
            await chnl.SendMessageAsync(message);
            Console.WriteLine(message);
        }

        public void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            var chnl = _discord.GetChannel(Settings.DiscordChannel) as IMessageChannel;
            chnl.SendMessageAsync($"[{DateTime.Now}] {message}").GetAwaiter().GetResult();
            Console.WriteLine(message);
        }

        public async Task LogAsync(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;
            var chnl = _discord.GetChannel(Settings.DiscordChannel) as IMessageChannel;
            await chnl.SendMessageAsync($"[{DateTime.Now}] {message}");
            Console.WriteLine(message);
        }
    }
}
