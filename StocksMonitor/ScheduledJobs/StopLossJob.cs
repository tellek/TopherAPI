using Alpaca.Markets;
using Discord;
using Discord.WebSocket;
using DiscordBot;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using static Contracts.AppSettings;

namespace StocksMonitor.ScheduledJobs
{
    public class StopLossJob : IJob
    {
        private RestClient client;
        private readonly DiscordCommands discord;

        public StopLossJob(DiscordCommands discord)
        {
            this.discord = discord;
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
