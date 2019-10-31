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
    public class RegularJob : IJob
    {
        private RestClient client;
        private readonly DiscordCommands discord;
        private readonly WatchSellJob watchSell;

        public RegularJob(DiscordCommands discord, WatchSellJob watchSell)
        {
            this.discord = discord;
            this.watchSell = watchSell;
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        public void Execute()
        {
            try
            {
                var positions = client.ListPositionsAsync().GetAwaiter().GetResult().ToList(); ;

                if (positions != null && positions.Count > 0)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine($"**Positions Update** [{DateTime.Now}] {watchSell.lossPercent}");
                    sb.AppendLine("```diff");
                    foreach (var pos in positions)
                    {
                        sb.AppendLine($"{ColorTextRedOrGreen(pos.UnrealizedProfitLoss)} {pos.Symbol} ${pos.AssetCurrentPrice} x{pos.Quantity} ${pos.MarketValue}" +
                            $" {addProfitLoss(pos.UnrealizedProfitLoss)} / {twoDecimalPlaces(pos.UnrealizedProfitLossPercent)}%");
                    }
                    sb.Append("```");
                    discord.Say(sb.ToString());
                }
            }
            catch (Exception e)
            {
                discord.Say($"**ERROR!** {e.Message}");
            }
        }

        private string addProfitLoss(decimal profitLoss)
        {
            if (profitLoss <= 0)
            {
                return $"{twoDecimalPlaces(profitLoss)}";
            }
            else
            {
                return $"+{twoDecimalPlaces(profitLoss)}";
            }
        }

        private string twoDecimalPlaces(decimal value)
        {
            return String.Format("{0:0.00}", value);
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
