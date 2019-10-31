using Alpaca.Markets;
using Contracts.StocksMonitor;
using DiscordBot;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rs = RestSharp;
using Newtonsoft.Json;
using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;
using System.Threading;

namespace StocksMonitor.Tasks
{
    public class HistoricalData
    {
        private readonly IConfiguration configuration;
        private readonly DiscordCommands discord;
        private readonly string connectionString;

        public HistoricalData(IConfiguration configuration, DiscordCommands discord)
        {
            this.configuration = configuration;
            this.discord = discord;
            connectionString = configuration["ConnectionStrings:MarketDataDb"];
        }

        public void Download(List<IAsset> filteredStocks)
        {
            foreach (var stock in filteredStocks)
            {
                long count = 0;
                var month = 10;
                while (month > 0)
                {
                    var data = GetData(stock.Symbol, 2019, month);
                    if (data.Status != "OK")
                    {
                        discord.Say($"!! {stock.Symbol} failed with status of {data.Status}.");
                    }
                    if (data.Results.Count() < 1)
                    {
                        month--;
                        continue;
                    }

                    var sql = new StringBuilder();
                    sql.AppendLine("INSERT INTO backtest.marketdata (ticker, datetime, volume, open, close, high, low) ");
                    sql.Append("VALUES ");

                    foreach (var d in data.Results)
                    {
                        sql.AppendLine($"('{data.Ticker}', '{EpochToDateTime(d.Time)}', {d.Volume}, {d.Open}, {d.Close}, {d.High}, {d.Low}),");
                    }

                    var finalSql = sql.ToString().Trim().Trim(',');

                    var DbConnection = new NpgsqlConnection(connectionString);
                    using (DbConnection)
                    {
                        DbConnection.Execute(finalSql);
                    }

                    count += data.QueryCount;
                    month--;
                    Thread.Sleep(5000);
                }

                discord.Say($"{count} records of candlestick data downloaded for {stock.Symbol}.");
            }
        }

        private DateTime EpochToDateTime(double epochTimeStamp)
        {
            // Java timestamp is milliseconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddMilliseconds(epochTimeStamp).ToUniversalTime();
            return dtDateTime;
        }

        private Candlesticks GetData(string ticker, int year, int month)
        {
            return GetData(ticker, new DateTime(year, month, 01), new DateTime(year, month + 1, 01));
        }

        private Candlesticks GetData(string ticker, DateTime from, DateTime to)
        {
            var epochFrom = from.ToString("yyyy-MM-dd");
            var epochTo = to.ToString("yyyy-MM-dd");

            var client = new Rs.RestClient($"{"https://"}api.polygon.io/v2/aggs/ticker/{ticker}/range/1/minute/{epochFrom}/{epochTo}");
            var request = new Rs.RestRequest(Method.GET);

            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Host", "api.polygon.io");
            request.AddQueryParameter("apiKey", "");

            IRestResponse response = client.Execute(request);
            return JsonConvert.DeserializeObject<Candlesticks>(response.Content);
        }

        internal IDbConnection Connection
        {
            get
            {
                return new NpgsqlConnection(connectionString);
            }
        }
    }
}
