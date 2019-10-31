using Alpaca.Markets;
using Contracts.StocksMonitor;
using Discord;
using Discord.WebSocket;
using DiscordBot;
using FluentScheduler;
using RestSharp;
using StocksMonitor.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using static Contracts.AppSettings;
using Rs = RestSharp;
using Am = Alpaca.Markets;
using Newtonsoft.Json;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Threading;

namespace StocksMonitor.ScheduledJobs
{
    public class ImmediateJob : IJob
    {
        private Am.RestClient client;
        private readonly DiscordCommands discord;
        private readonly WatchSellJob watchSell;
        private readonly RegularJob regularJob;
        private readonly GetFilteredStocks getFilteredStocks;
        private readonly GetAggregateData getAggregateData;
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        public ImmediateJob(DiscordCommands discord, WatchSellJob watchSell, RegularJob regularJob,
            GetFilteredStocks getFilteredStocks, GetAggregateData getAggregateData, IConfiguration configuration)
        {
            this.discord = discord;
            this.watchSell = watchSell;
            this.regularJob = regularJob;
            this.getFilteredStocks = getFilteredStocks;
            this.getAggregateData = getAggregateData;
            this.configuration = configuration;
            client = new Am.RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
            connectionString = configuration["ConnectionStrings:MarketDataDb"];
        }

        public void Execute()
        {
            discord.Log("Starting up...");

            
        }


    }
}