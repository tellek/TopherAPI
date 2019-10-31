using Alpaca.Markets;
using Contracts.StocksMonitor;
using Discord;
using Discord.WebSocket;
using DiscordBot;
using FluentScheduler;
using StocksMonitor.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using static Contracts.AppSettings;

namespace StocksMonitor.ScheduledJobs
{
    public class AfternoonJob : IJob
    {
        private readonly DiscordCommands discord;
        private readonly GetFilteredStocks getFilteredStocks;
        private readonly GetAggregateData getAggregateData;
        private readonly ManageAssets manageAssets;
        private RestClient client;

        public AfternoonJob(DiscordCommands discord, GetFilteredStocks getFilteredStocks, GetAggregateData getAggregateData, ManageAssets manageAssets)
        {
            this.discord = discord;
            this.getFilteredStocks = getFilteredStocks;
            this.getAggregateData = getAggregateData;
            this.manageAssets = manageAssets;
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        public void Execute()
        {
            try
            {
                var hoursBack = 48;
                var keepPercent = 1.5m;

                var startingStocks = getFilteredStocks.Execute(client);
                var aggStocks = getAggregateData.Execute(startingStocks, hoursBack);

                var stocks = new Dictionary<string, AssetAgg>();
                foreach (var agg in aggStocks)
                {
                    var price1 = agg.Value.Agg.First().Open;
                    var price2 = agg.Value.Agg.Last().Close;
                    var percent = Maths.PercentDiff(agg.Value.Agg.First().Open, agg.Value.Agg.Last().Close);
                    if (percent >= keepPercent) stocks.Add(agg.Key, agg.Value);
                }

                ScoreStocks(stocks);

                var sb = new StringBuilder();
                foreach (var memStk in Memory.RememberedStocks)
                {
                    sb.Append($"**{memStk.Key}**:{memStk.Value.Score}, ");
                }
                discord.Say(StringHelpers.LimitStringToCount(sb.ToString()));

                client.DeleteAllOrdersAsync().GetAwaiter().GetResult();
                Memory.CurrentOrders.Clear();
                var positions = client.ListPositionsAsync().GetAwaiter().GetResult().ToList();

                manageAssets.SellStocks(client, positions);
                manageAssets.BuyStocks(client, positions);
            }
            catch (Exception e)
            {
                discord.Say($"**ERROR!** {e.Message}");
            }
        }

        private void ScoreStocks(Dictionary<string, AssetAgg> stocks)
        {
            // Increase score based on how close it stays to a straight upward line.
            var upAllowance = 0.5m;
            var downAllowance = -0.5m;

            foreach (var st in stocks)
            {
                st.Value.OpenCloseDiff = st.Value.Agg.Last().Close - st.Value.Agg.First().Open;
                st.Value.PointTotal = st.Value.Agg.Count;
                st.Value.IncreasePerPoint = st.Value.OpenCloseDiff / st.Value.PointTotal;
                st.Value.DiffPercent = Maths.PercentDiff(st.Value.Agg.First().Open, st.Value.Agg.Last().Close);
                st.Value.percentPerPoint = st.Value.DiffPercent / st.Value.PointTotal;

                int count = 0;
                decimal savedPrice = st.Value.Agg.First().Open;
                var changes = new List<decimal>();
                foreach (var ag in st.Value.Agg)
                {
                    count++;

                    var flux = st.Value.Agg.First().Open + (st.Value.IncreasePerPoint * count);
                    var percent = Maths.PercentDiff(flux, ag.Close);

                    var chng = percent - st.Value.percentPerPoint;

                    if (percent >= downAllowance && percent <= upAllowance)
                    {
                        changes.Add(percent);
                    }

                    savedPrice = ag.Close;
                }
                st.Value.Score = changes.Count;
            }
            Memory.RememberedStocks = stocks.OrderByDescending(x => x.Value.Score).ToDictionary(y => y.Key, z => z.Value);
        }
    }

    
}