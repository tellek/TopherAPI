using Alpaca.Markets;
using Contracts.StocksMonitor;
using Discord;
using Discord.WebSocket;
using DiscordBot;
using FluentScheduler;
using StocksMonitor.Tasks;
using System;
using System.Text;
using Utilities;
using static Contracts.AppSettings;

namespace StocksMonitor.ScheduledJobs
{
    public class NightlyJob : IJob
    {
        private RestClient client;
        private readonly DiscordCommands discord;
        private readonly ManageAssets manage;
        private readonly WatchSellJob watchSell;

        public NightlyJob(DiscordCommands discord, ManageAssets manage, WatchSellJob watchSell)
        {
            this.discord = discord;
            this.manage = manage;
            this.watchSell = watchSell;
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        public void Execute()
        {
            watchSell.lossPercent = -0.01m;
            //if (Settings.ServiceHasBeenDisabled) return;
            //try
            //{
            //    manage.CancelOrders(client);

            //    if (Memory.OwnedStocks == null || Memory.OwnedStocks.Count <= 0) return;
            //    foreach (var os in Memory.OwnedStocks)
            //    {
            //        var stopDifference = Maths.Percent(os.Value.PricePer, 3);
            //        var stopPrice = os.Value.PricePer - stopDifference;
            //        var result = client.PostOrderAsync(os.Key, os.Value.Amount, OrderSide.Sell, OrderType.Stop, TimeInForce.Day,
            //            stopPrice: stopPrice).GetAwaiter().GetResult();
            //        os.Value.OrderNumber = result.OrderId;
            //        discord.Say($"Stop loss for {os.Value.Amount} shares of {os.Key} set at ${stopPrice} each.");
            //    }
            //}
            //catch (Exception e)
            //{
            //    discord.Say($"**ERROR!** {e.Message}");
            //}

        }

    }
}
