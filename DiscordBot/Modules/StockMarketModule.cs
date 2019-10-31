using Contracts;
using Contracts.StocksMonitor;
using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace DiscordBot.Modules
{
    [Name("StockMarket")]
    [Summary("Commands to interract with the stock market through Alpaca.")]
    public class StockMarketModule : ModuleBase<SocketCommandContext>
    {
        IMarketInformation _market;

        public StockMarketModule(IMarketInformation market)
        {
            _market = market;
        }

        [Command("market.toggle"), Alias("market.t", "m.toggle", "m.t")]
        [Summary("Toggle the process that runs every 15 seconds on and off.")]
        public async Task Toggle()
        {
            await Context.Message.DeleteAsync();

            if (AppSettings.Settings.ServiceHasBeenDisabled)
            {
                AppSettings.Settings.ServiceHasBeenDisabled = false;
                await ReplyAsync("Enabled constant process.");
            }
            else
            {
                AppSettings.Settings.ServiceHasBeenDisabled = true;
                await ReplyAsync("Disabled constant process.");
            }
        }

        [Command("market.clocks"), Alias("market.c", "m.clocks", "m.c")]
        [Summary("Get the current marketplace clock data.")]
        public async Task Clocks()
        {
            var result = _market.GetMarketClockData();
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Title = "Stock Market Time Data"
            };

            var props = ObjectParsing.AllPropsToDictionary(result);

            foreach (var prop in props)
            {
                builder.AddField(x =>
                {
                    x.Name = prop.Key;
                    x.Value = prop.Value.ToString();
                    x.IsInline = true;
                });
            }

            await ReplyAsync(embed: builder.Build());
        }

        [Command("market.account"), Alias("market.a", "m.account", "m.a")]
        [Summary("Get the current Alpaca account data.")]
        public async Task Account()
        {
            await Context.Message.DeleteAsync();

            var result = _market.GetAlpacaAccountData();
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Title = "Alpaca Account Information"
            };
            builder.AddField(x =>
            {
                x.Name = "Status";
                x.Value = result.Status;
                x.IsInline = true;
            });
            builder.AddField(x =>
            {
                x.Name = "Buying Power";
                x.Value = result.BuyingPower;
                x.IsInline = true;
            });
            builder.AddField(x =>
            {
                x.Name = "Current Equity";
                x.Value = result.Equity;
                x.IsInline = true;
            });
            builder.AddField(x =>
            {
                x.Name = "Tradable Cash";
                x.Value = result.TradableCash;
                x.IsInline = true;
            });
            builder.AddField(x =>
            {
                x.Name = "Withdrawable Cash";
                x.Value = result.WithdrawableCash;
                x.IsInline = true;
            });
            builder.AddField(x =>
            {
                x.Name = "DayTade Marks";
                x.Value = result.DayTradeCount;
                x.IsInline = true;
            });


            await ReplyAsync(embed: builder.Build());
        }

        [Command("market.orders"), Alias("market.o", "m.orders", "m.o")]
        [Summary("Get all orders that have been placed but not filled.")]
        public async Task Orders()
        {
            await Context.Message.DeleteAsync();
            var results = _market.GetMarketOrderData().ToList();

            if (results.Count < 1)
            {
                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Title = "Alpaca Market Orders",
                    Description = "No orders found."
                };
                await ReplyAsync(embed: builder.Build());
            }
            else
            {
                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Title = "Alpaca Market Orders"
                };

                foreach (var result in results)
                {
                    builder.AddField(x =>
                    {
                        x.Name = "Symbol";
                        x.Value = result.Symbol;
                        x.IsInline = true;
                    });
                    builder.AddField(x =>
                    {
                        x.Name = "Date";
                        x.Value = result.CreatedAt;
                        x.IsInline = true;
                    });
                    builder.AddField(x =>
                    {
                        x.Name = "Quantity";
                        x.Value = result.Quantity;
                        x.IsInline = true;
                    });
                    builder.AddField(x =>
                    {
                        x.Name = "Type";
                        x.Value = result.OrderType;
                        x.IsInline = true;
                    });
                    builder.AddField(x =>
                    {
                        x.Name = "Limit";
                        x.Value = result.LimitPrice;
                        x.IsInline = true;
                    });
                    builder.AddField(x =>
                    {
                        x.Name = "Stop";
                        x.Value = result.StopPrice;
                        x.IsInline = true;
                    });
                }

                await ReplyAsync(embed: builder.Build());
            }
            
        }

        [Command("market.positions"), Alias("market.p", "m.positions", "m.p")]
        [Summary("Get position information for all current assets.")]
        public async Task Positions()
        {
            await Context.Message.DeleteAsync();
            var results = _market.GetMarketPositionData().ToList();

            if (results.Count < 1)
            {
                await ReplyAsync("No positions found.");
            }
            else
            {
                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Title = "Alpaca Market Positions"
                };

                foreach (var result in results)
                {
                    builder.AddField(x =>
                    {
                        x.Name = "Symbol";
                        x.Value = result.Symbol;
                        x.IsInline = true;
                    });
                    builder.AddField(x =>
                    {
                        x.Name = "Quantity";
                        x.Value = result.Quantity;
                        x.IsInline = true;
                    });
                    builder.AddField(x =>
                    {
                        x.Name = "Price";
                        x.Value = result.AssetCurrentPrice;
                        x.IsInline = true;
                    });
                }

                await ReplyAsync(embed: builder.Build());
            }

        }

        [Command("market.healthiest"), Alias("market.h", "m.healthiest", "m.h")]
        [Summary("Get the current healthiest stocks on the marketplace.")]
        public async Task GetHealthiest()
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync("Working on it...");
            var result = _market.GetHealthiestStocks();

            if (result.Count > 0)
            {
                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Title = "Healthiest Stocks"
                };

                string prevSymbol = "";
                decimal prevScore = 0;
                decimal prevPrice = 0;
                int count = 1;

                foreach (var r in result)
                {
                    if (count == 1)
                    {
                        builder.AddField(x =>
                        {
                            x.Name = "Symbol";
                            x.Value = r.Key;
                            x.IsInline = true;
                        });
                        builder.AddField(x =>
                        {
                            x.Name = "Score";
                            x.Value = r.Value.Item1;
                            x.IsInline = true;
                        });
                        builder.AddField(x =>
                        {
                            x.Name = "Price";
                            x.Value = r.Value.Item2.Price;
                            x.IsInline = true;
                        });
                    }
                    else if (count % 2 == 0)
                    {
                        prevSymbol = r.Key;
                        prevScore = r.Value.Item1;
                        prevPrice = r.Value.Item2.Price;
                    }
                    else
                    {
                        builder.AddField(x =>
                        {
                            x.Name = prevSymbol;
                            x.Value = r.Key;
                            x.IsInline = true;
                        });
                        builder.AddField(x =>
                        {
                            x.Name = prevScore.ToString();
                            x.Value = r.Value.Item1;
                            x.IsInline = true;
                        });
                        builder.AddField(x =>
                        {
                            x.Name = prevPrice.ToString();
                            x.Value = r.Value.Item2.Price;
                            x.IsInline = true;
                        });
                    }
                    count++;
                }
                await ReplyAsync(embed: builder.Build());
            }
            else
            {
                var builder = new EmbedBuilder()
                {
                    Color = new Color(114, 137, 218),
                    Title = "Healthiest Stocks",
                    Description = "The get healthiest stocks process could not find any results at this time."
                };
                await ReplyAsync(embed: builder.Build());
            }

        }
    }
}
