using Alpaca.Markets;
using Discord;
using Discord.WebSocket;
using DiscordBot;
using FluentScheduler;
using StocksMonitor.Processes;
using System;
using System.Diagnostics;
using System.Linq;
using Utilities;
using static Contracts.AppSettings;

namespace StocksMonitor.ScheduledJobs
{
    public class MorningJob : IJob
    {
        private RestClient client;
        private readonly DiscordCommands discord;

        public MorningJob(DiscordCommands discord)
        {
            this.discord = discord;
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        public void Execute()
        {
            Memory.StoredAssets.Clear();
            discord.Say($"Downloading and filtering assets...");
            var sw = Stopwatch.StartNew();
            var ac = new AssetCollection();
            ac.FilterIfLosingValue = false;
            ac.MaxPrice = 10.00m;
            ac.MinPrice = 2.00m;
            Memory.StoredAssets = ac.GetFilteredAssetsAsConcurrentDictionary();
            discord.Say($"{Memory.StoredAssets.Count} assets stored in memory. ({sw.Elapsed.TotalSeconds} seconds)");
            sw.Stop();

            //try
            //{
            //    client.DeleteAllOrdersAsync().GetAwaiter().GetResult();

            //    var positions = client.ListPositionsAsync().GetAwaiter().GetResult().ToList();
            //    if (positions == null || positions.Count <= 0)
            //    {
            //        discord.Say($"No stock assets are owned at this time.");
            //        return;
            //    }
            //    foreach (var pos in positions)
            //    {
            //        if (Memory.OwnedStocks.ContainsKey(pos.Symbol))
            //        {
            //            var amount = Memory.OwnedStocks[pos.Symbol].Amount;
            //            var price = Memory.OwnedStocks[pos.Symbol].PricePer;
            //            var stopDifference = Maths.Percent(price, 0.2m);
            //            var stopPrice = price - stopDifference;

            //            if (amount <= 0 || price <= 0) continue;
            //            var result = client.PostOrderAsync(
            //                pos.Symbol, 
            //                amount, 
            //                OrderSide.Sell, 
            //                OrderType.Stop, 
            //                TimeInForce.Day, 
            //                stopPrice: stopPrice).GetAwaiter().GetResult();
            //            discord.Say($"Stop loss for {amount} shares of {pos.Symbol} set at ${stopPrice} each.");
            //        }
            //    }
            //}
            //catch (Exception e)
            //{
            //    discord.Say($"**ERROR!** {e.Message}");
            //}

        }

    }
}
