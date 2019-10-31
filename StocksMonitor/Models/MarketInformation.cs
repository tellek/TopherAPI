using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksMonitor.Models
{
    public class MarketInformation
    {
        public IClock ClockData { get; set; }
        public IAccount AccountInfo { get; set; }
        public IEnumerable<IOrder> Orders { get; set; }
        public IEnumerable<IPosition> Positions { get; set; }
    }
}
