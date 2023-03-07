using Alpaca.Markets;
using DiscordBot;
using System;
using System.Linq;
using Am = Alpaca.Markets;

namespace StocksMonitor.Tasks
{
    public class MarketTasks
    {
        private Am.RestClient client;
        private DiscordCommands discord;

        public MarketTasks(RestClient client, DiscordCommands discord)
        {
            this.discord = discord;
            this.client = client;
        }

        public DateTime GetNextMarketCloseUtc()
        {
            var calendars = client.ListCalendarAsync(DateTime.Today).GetAwaiter().GetResult();
            var closingTime = calendars.First().TradingCloseTime;

            discord.Log($"Next Market Close: {closingTime.ToShortDateString()} {closingTime.ToLongTimeString()}");

            return closingTime;
        }

        public DateTime GetNextMarketOpenUtc()
        {
            var calendars = client.ListCalendarAsync(DateTime.Today).GetAwaiter().GetResult();
            var openingTime = calendars.First().TradingOpenTime;

            discord.Log($"Next Market Open: {openingTime.ToShortDateString()} {openingTime.ToLongTimeString()}");

            return openingTime;
        }
    }
}
