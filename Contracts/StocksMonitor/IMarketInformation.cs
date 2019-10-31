using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.StocksMonitor
{
    public interface IMarketInformation
    {
        IClock GetMarketClockData();

        IAccount GetAlpacaAccountData();

        IEnumerable<IOrder> GetMarketOrderData();

        IEnumerable<IPosition> GetMarketPositionData();

        Dictionary<string, (decimal, ILastTrade)> GetHealthiestStocks();
    }
}
