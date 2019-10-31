using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts.StocksMonitor
{
    public partial class Candlesticks
    {
        [JsonProperty("ticker")]
        public string Ticker { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("queryCount")]
        public long QueryCount { get; set; }

        [JsonProperty("resultsCount")]
        public long ResultsCount { get; set; }

        [JsonProperty("adjusted")]
        public bool Adjusted { get; set; }

        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }

    public partial class Result
    {
        [JsonProperty("v")]
        public long Volume { get; set; }

        [JsonProperty("o")]
        public double Open { get; set; }

        [JsonProperty("c")]
        public double Close { get; set; }

        [JsonProperty("h")]
        public double High { get; set; }

        [JsonProperty("l")]
        public double Low { get; set; }

        [JsonProperty("t")]
        public long Time { get; set; }

        [JsonProperty("n")]
        public long Number { get; set; }
    }
}
