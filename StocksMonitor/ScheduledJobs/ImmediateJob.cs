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
using StocksMonitor.Processes;
using System.Diagnostics;

namespace StocksMonitor.ScheduledJobs
{
    public class ImmediateJob : IJob
    {
        private Am.RestClient restClient;
        private readonly DiscordCommands discord;
        private readonly WatchSellJob watchSell;
        private readonly RegularJob regularJob;
        private readonly GetFilteredStocks getFilteredStocks;
        private readonly GetAggregateData getAggregateData;
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        private string symbol = "SPY";
        private Decimal scale = 200;

        private Guid lastTradeId = Guid.NewGuid();
        private List<Decimal> closingPrices = new List<Decimal>();

        public ImmediateJob(DiscordCommands discord, WatchSellJob watchSell, RegularJob regularJob,
            GetFilteredStocks getFilteredStocks, GetAggregateData getAggregateData, IConfiguration configuration)
        {
            this.discord = discord;
            this.watchSell = watchSell;
            this.regularJob = regularJob;
            this.getFilteredStocks = getFilteredStocks;
            this.getAggregateData = getAggregateData;
            this.configuration = configuration;
            restClient = new Am.RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
            connectionString = configuration["ConnectionStrings:MarketDataDb"];
        }

        public void Execute()
        {
            var account = new AccountTasks(restClient, discord);
            var market = new MarketTasks(restClient, discord);

            account.CancelOrders();

            var closeUtc = market.GetNextMarketCloseUtc();
            var openUtc = market.GetNextMarketOpenUtc();
            
        }


    }

}