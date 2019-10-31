using Alpaca.Markets;
using Contracts.StocksMonitor;
using StocksMonitor.Processes;
using System;
using System.Collections.Generic;
using static Contracts.AppSettings;

namespace StocksMonitor.Engines
{
    public class GetMarketInformation : IMarketInformation
    {
        private RestClient client;
        private readonly HealthiestStocks _hStocks;

        public GetMarketInformation(HealthiestStocks hStocks)
        {
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
            _hStocks = hStocks;
        }

        public IClock GetMarketClockData()
        {
            return client.GetClockAsync().Result;
        }

        public IAccount GetAlpacaAccountData()
        {
            return client.GetAccountAsync().Result;
        }

        public IEnumerable<IOrder> GetMarketOrderData()
        {
            return client.ListOrdersAsync().Result;
        }

        public IEnumerable<IPosition> GetMarketPositionData()
        {
            return client.ListPositionsAsync().Result;
        }

        public Dictionary<string, (decimal, ILastTrade)> GetHealthiestStocks()
        {
            return _hStocks.FindThem();
        }
    }
}