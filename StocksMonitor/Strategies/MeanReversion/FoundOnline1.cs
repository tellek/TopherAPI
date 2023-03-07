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

namespace StocksMonitor.Strategies.MeanReversion
{
    public class FoundOnline1 : IJob
    {
        private Am.RestClient restClient;
        private readonly DiscordCommands discord;
        private readonly GetFilteredStocks getFilteredStocks;
        private readonly GetAggregateData getAggregateData;
        private readonly IConfiguration configuration;
        private readonly string connectionString;

        private string symbol = "SPY";
        private Decimal scale = 200;

        private Guid lastTradeId = Guid.NewGuid();
        private List<Decimal> closingPrices = new List<Decimal>();

        public FoundOnline1(DiscordCommands discord, GetFilteredStocks getFilteredStocks, 
            GetAggregateData getAggregateData, IConfiguration configuration)
        {
            this.discord = discord;
            this.getFilteredStocks = getFilteredStocks;
            this.getAggregateData = getAggregateData;
            this.configuration = configuration;
            restClient = new Am.RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
            connectionString = configuration["ConnectionStrings:MarketDataDb"];
        }

        public void Execute()
        {
            discord.Log("Starting up...");

            // First, cancel any existing orders so they don't impact our buying power.
            var orders = restClient.ListOrdersAsync().Result;
            foreach (var order in orders)
            {
                restClient.DeleteOrderAsync(order.OrderId);
                discord.Log($"Order {order.Symbol}:{order.OrderId} cancelled.");
            }

            // Figure out when the market will close so we can prepare to sell beforehand.
            var calendars = restClient.ListCalendarAsync(DateTime.Today).Result;
            var calendarDate = calendars.First().TradingDate;
            var closingTime = calendars.First().TradingCloseTime;
            closingTime = new DateTime(calendarDate.Year, calendarDate.Month, calendarDate.Day, closingTime.Hour,
                closingTime.Minute, closingTime.Second);

            discord.Log($"Next ClosingTime: {closingTime.ToShortDateString()} {closingTime.ToLongTimeString()}");

            discord.Log("Waiting for market open...");
            AwaitMarketOpen();
            discord.Log("Market opened.");

            // Check every minute for price updates.
            TimeSpan timeUntilClose = closingTime - DateTime.UtcNow;
            discord.Log($"{closingTime} - {DateTime.UtcNow} = {closingTime - DateTime.UtcNow}");
            discord.Log($"{timeUntilClose.TotalMinutes} > 15 = {timeUntilClose.TotalMinutes > 15}");
            while (timeUntilClose.TotalMinutes > 15)
            {
                // Cancel old order if it's not already been filled.
                restClient.DeleteOrderAsync(lastTradeId);

                // Get information about current account value.
                var account = restClient.GetAccountAsync().Result;
                Decimal buyingPower = account.BuyingPower;
                Decimal portfolioValue = account.Equity;

                // Get information about our existing position.
                int positionQuantity = 0;
                Decimal positionValue = 0;
                try
                {
                    var currentPosition = restClient.GetPositionAsync(symbol).Result;
                    positionQuantity = currentPosition.Quantity;
                    positionValue = currentPosition.MarketValue;
                }
                catch (Exception e)
                {
                    // No position exists. This exception can be safely ignored.
                }

                var barSet = restClient.GetBarSetAsync(new String[] { symbol }, TimeFrame.Minute, 20).Result;
                var bars = barSet[symbol];
                Decimal avg = bars.Average(item => item.Close);
                Decimal currentPrice = bars.Last().Close;
                Decimal diff = avg - currentPrice;

                if (diff <= 0)
                {
                    // Above the 20 minute average - exit any existing long position.
                    if (positionQuantity > 0)
                    {
                        Console.WriteLine("Setting position to zero.");
                        SubmitOrder(positionQuantity, currentPrice, OrderSide.Sell);
                    }
                    else
                    {
                        Console.WriteLine("No position to exit.");
                    }
                }
                else
                {
                    // Allocate a percent of our portfolio to this position.
                    Decimal portfolioShare = diff / currentPrice * scale;
                    Decimal targetPositionValue = portfolioValue * portfolioShare;
                    Decimal amountToAdd = targetPositionValue - positionValue;

                    if (amountToAdd > 0)
                    {
                        // Buy as many shares as we can without going over amountToAdd.

                        // Make sure we're not trying to buy more than we can.
                        if (amountToAdd > buyingPower)
                        {
                            amountToAdd = buyingPower;
                        }
                        int qtyToBuy = (int)(amountToAdd / currentPrice);

                        SubmitOrder(qtyToBuy, currentPrice, OrderSide.Buy);
                    }
                    else
                    {
                        // Sell as many shares as we can without going under amountToAdd.

                        // Make sure we're not trying to sell more than we have.
                        amountToAdd *= -1;
                        int qtyToSell = (int)(amountToAdd / currentPrice);
                        if (qtyToSell > positionQuantity)
                        {
                            qtyToSell = positionQuantity;
                        }

                        SubmitOrder(qtyToSell, currentPrice, OrderSide.Sell);
                    }
                }

                Thread.Sleep(60000);
                Console.WriteLine($"{closingTime} - {DateTime.UtcNow} = {closingTime - DateTime.UtcNow}");
                Console.WriteLine($"{timeUntilClose.TotalMinutes} > 15 = {timeUntilClose.TotalMinutes > 15}");
                timeUntilClose = closingTime - DateTime.Now;
            }

            discord.Log("Market nearing close.");
            ClosePositionAtMarket();
        }

        private void AwaitMarketOpen()
        {
            while (!restClient.GetClockAsync().Result.IsOpen)
            {
                Thread.Sleep(60000);
            }
        }

        // Submit an order if quantity is not zero.
        private void SubmitOrder(int quantity, Decimal price, OrderSide side)
        {
            if (quantity == 0)
            {
                Console.WriteLine("No order necessary.");
                return;
            }
            discord.Log($"Submitting {side} order for {quantity} shares of {symbol} at ${price}.");
            var order = restClient.PostOrderAsync(symbol, quantity, side, OrderType.Limit, TimeInForce.Day, price).Result;
            lastTradeId = order.OrderId;
        }

        private void ClosePositionAtMarket()
        {
            try
            {
                var positionQuantity = restClient.GetPositionAsync(symbol).Result.Quantity;
                discord.Log($"Closing position sell order for {positionQuantity} shares of {symbol} at market value.");
                restClient.PostOrderAsync(symbol, positionQuantity, OrderSide.Sell, OrderType.Market, TimeInForce.Day);
            }
            catch (Exception e)
            {
                // No position to exit.
            }
        }
    }

}