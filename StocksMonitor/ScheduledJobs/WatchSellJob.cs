﻿using Alpaca.Markets;
using Discord;
using Discord.WebSocket;
using DiscordBot;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Utilities;
using static Contracts.AppSettings;

namespace StocksMonitor.ScheduledJobs
{
    public class WatchSellJob : IJob
    {
        private RestClient client;
        private readonly DiscordCommands discord;
        
        private decimal sellSubtractionPercent = 0.01m;

        private decimal profitPercent = 0.02m;
        public decimal lossPercent = -0.01m;

        private decimal lossPercentGain = 0.00006666m;

        public WatchSellJob(DiscordCommands discord)
        {
            this.discord = discord;
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        public void Execute()
        {
            if (Settings.ServiceHasBeenDisabled) return;

            var aggregates = new Dictionary<string, IEnumerable<IAgg>>();
            var groups = ConvertListToGroupsOfSymbols();
            foreach (var g in groups)
            {
                var groupResult = client.GetBarSetAsync(g, TimeFrame.Minute, 2).GetAwaiter().GetResult();
                foreach (var gr in groupResult)
                {
                    aggregates.Add(gr.Key, gr.Value);
                }
            }

            var possible = new Dictionary<string, IEnumerable<IAgg>>();
            (string symbol, decimal value) highest = ("",0);

            foreach (var agg in aggregates)
            {
                if (agg.Value.Count() < 2) continue;
                var test = Utilities.Maths.PercentDiff(agg.Value.First().Volume, agg.Value.Last().Volume);
                //if (test > 500) possible.Add(agg.Key, agg.Value);
                if (test > highest.value) highest = (agg.Key, test);
            }

            discord.Say($"{highest.symbol} -> +{highest.value}%");
            //foreach (var item in possible)
            //{
            //    discord.Say($"You may want to investigate {item.Key}.");
            //}




            //if (lossPercent < 0) lossPercent += lossPercentGain;
            //if (lossPercent > 0) lossPercent = 0;

            //try
            //{
            //    var positions = client.ListPositionsAsync().GetAwaiter().GetResult().ToList();
            //    if (positions == null || positions.Count <= 0) return;
            //    client.DeleteAllOrdersAsync().GetAwaiter().GetResult();
            //    Thread.Sleep(100);

            //    foreach (var pos in positions)
            //    {
            //        if (pos.UnrealizedProfitLossPercent > profitPercent)
            //        {
            //                var sellDifference = Maths.Percent(pos.AssetCurrentPrice, sellSubtractionPercent);
            //                var sellPrice = pos.AssetCurrentPrice - sellDifference;
            //                var sellResult = client.PostOrderAsync(pos.Symbol, pos.Quantity, OrderSide.Sell, OrderType.Market, TimeInForce.Day).GetAwaiter().GetResult();
            //                discord.Say(BroadcastSellResult(pos.UnrealizedProfitLoss,
            //                    //$"Sold {pos.Quantity} of {pos.Symbol} for ${sellPrice} each at a total of ${sellPrice * pos.Quantity}. " +
            //                    //$"({pos.AssetCurrentPrice}-{sellDifference}={sellPrice})"));
            //                    $"{pos.Quantity} of {pos.Symbol} at ${pos.AssetCurrentPrice} for approximately ${pos.AssetCurrentPrice * pos.Quantity}."));
            //                Memory.JustSoldStocks.Add(pos.Symbol);
            //        }

            //        if (pos.UnrealizedProfitLossPercent < lossPercent)
            //        {
            //                var sellResult = client.PostOrderAsync(pos.Symbol, pos.Quantity, OrderSide.Sell, OrderType.Market, TimeInForce.Day).GetAwaiter().GetResult();
            //                discord.Say(BroadcastSellResult(pos.UnrealizedProfitLoss,
            //                    //$"Sold {pos.Quantity} of {pos.Symbol} for ${pos.AssetCurrentPrice} each at a total of ${pos.AssetCurrentPrice * pos.Quantity}."));
            //                    $"{pos.Quantity} of {pos.Symbol} at ${pos.AssetCurrentPrice} for approximately ${pos.AssetCurrentPrice * pos.Quantity}."));
            //                Memory.JustSoldStocks.Add(pos.Symbol);
            //        }
            //    }

            //}
            //catch (Exception e)
            //{
            //    discord.Say($"**ERROR!** {e.Message} - {e.InnerException.Message}");
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

        private string BroadcastSellResult(decimal value, string message)
        {
            var sb = new StringBuilder();
            sb.AppendLine("```diff");
            sb.AppendLine("SOLD");
            sb.AppendLine($"{ColorTextRedOrGreen(value)} {message}");
            sb.Append("```");
            return sb.ToString();
        }

        private string ColorTextRedOrGreen(decimal profitLoss)
        {
            if (profitLoss < 0)
            {
                return $"-";
            }
            else if (profitLoss > 0)
            {
                return $"+";
            }
            else
            {
                return $" ";
            }
        }
    }
}
