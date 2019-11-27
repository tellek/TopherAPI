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

            //Memory.StoredAssets.Clear();
            //discord.Say($"Downloading and filtering assets...");
            //var sw = Stopwatch.StartNew();
            //var ac = new AssetCollection();
            //ac.MaxPrice = 50.00m;
            //ac.MinPrice = 10.00m;
            //ac.FilterIfLosingValue = false;
            //Memory.StoredAssets = ac.GetFilteredAssetsAsConcurrentDictionary();
            //discord.Say($"{Memory.StoredAssets.Count} assets stored in memory. ({sw.Elapsed.TotalSeconds} seconds)");
            //sw.Stop();

            //var aggregates = new Dictionary<string, IEnumerable<IAgg>>();
            //var groups = ConvertListToGroupsOfSymbols();
            //foreach (var g in groups)
            //{
            //    var groupResult = client.GetBarSetAsync(g, TimeFrame.Minute, 2).GetAwaiter().GetResult();
            //    foreach (var gr in groupResult)
            //    {
            //        aggregates.Add(gr.Key, gr.Value);
            //    }
            //}

            //var possible = new Dictionary<string, IEnumerable<IAgg>>();

            //foreach (var agg in aggregates)
            //{
            //    if (agg.Value.Count() < 2) continue;
            //    var test = Utilities.Maths.PercentDiff(agg.Value.First().Volume, agg.Value.Last().Volume);
            //    if (test > 500) possible.Add(agg.Key, agg.Value);
            //}

            //foreach (var item in possible)
            //{
            //    discord.Say($"You may want to investigate {item.Key}.");
            //}
        }

        private List<List<string>> ConvertListToGroupsOfSymbols(int groupSize = 100)
        {
            var assets = new List<string>();
            foreach (var asset in Memory.StoredAssets)
            {
                assets.Add(asset.Key);
            }

            var AssetsGroups = new List<List<string>>();
            int count = 0;
            for (int i = 0; i < assets.Count; i += groupSize)
            {
                var temp = assets.GetRange(i, Math.Min(groupSize, assets.Count - i));
                AssetsGroups.Add(new List<string>());
                foreach (var item in temp)
                {
                    AssetsGroups[count].Add(item);
                }
                count++;
            }
            return AssetsGroups;
        }
    }

}