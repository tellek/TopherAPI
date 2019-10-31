using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.StocksMonitor
{
    public class OwnedAsset
    {
        public long Amount { get; set; }
        public decimal PricePer { get; set; }
        public Guid OrderNumber { get; set; }
    }
}
