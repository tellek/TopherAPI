using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.StocksMonitor
{
    public class AssetAgg
    {
        public List<IAgg> Agg { get; set; }

        /// <summary>
        /// A value designating how close this stock is to trending and even line upward. (higher is better)
        /// </summary>
        public decimal Score { get; set; }

        /// <summary>
        /// The difference between the first open price and the last close price.
        /// </summary>
        public decimal OpenCloseDiff { get; set; }

        /// <summary>
        /// The total amount of points recorded in the aggregate timeline.
        /// </summary>
        public int PointTotal { get; set; }

        /// <summary>
        /// The average amount that should be increased each point to make a perfect upward trend across all points.
        /// </summary>
        public decimal IncreasePerPoint { get; set; }

        /// <summary>
        /// The percentage change that was made from first open to last close.
        /// </summary>
        public decimal DiffPercent { get; set; }

        /// <summary>
        /// The average percentage that the price should increase across all points for a perfect upward trend.
        /// </summary>
        public decimal percentPerPoint { get; set; }
        

        public decimal FluxPercent { get; set; }
        
    }
}
