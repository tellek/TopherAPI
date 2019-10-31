using Contracts.StocksMonitor;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksMonitor
{
    public static class Memory
    {
        public static Dictionary<string, AssetAgg> RememberedStocks = new Dictionary<string, AssetAgg>();
        public static Dictionary<string, Guid> CurrentOrders = new Dictionary<string, Guid>();
        public static List<string> JustSoldStocks = new List<string>();
    }
}
