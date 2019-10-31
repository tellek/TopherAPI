using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using static Contracts.AppSettings;
using RestSharp;
using Newtonsoft.Json;

namespace StocksMonitor.Tasks
{
    public class GetFilteredStocks
    {
        private Alpaca.Markets.RestClient _client;
        private const int GroupSize = 100;
        private const int MinLastVolume = 500000;
        private const decimal MaxWatchPrice = 10.00m;
        private const decimal MinWatchPrice = 5.00m;

        public GetFilteredStocks()
        {
            
        }

        public List<IAsset> Execute(Alpaca.Markets.RestClient client)
        {
            _client = client;

            var assets = _client.ListAssetsAsync().GetAwaiter().GetResult().ToList();
            //var assets = CallPolygonApi();
            FilterAssets(ref assets);

            var groups = ConvertListToGroupsOfSymbols(assets);
            NarrowDownAssetsByRules(groups, ref assets);

            return assets;
        }

        private List<List<string>> ConvertListToGroupsOfSymbolsAlt(List<PolygonTicker> assets)
        {
            var AssetsGroups = new List<List<string>>();
            int count = 0;
            for (int i = 0; i < assets.Count; i += GroupSize)
            {
                var temp = assets.GetRange(i, Math.Min(GroupSize, assets.Count - i));
                AssetsGroups.Add(new List<string>());
                foreach (var item in temp)
                {
                    AssetsGroups[count].Add(item.ticker);
                }
                count++;
            }
            return AssetsGroups;
        }

        private void NarrowDownAssetsByRulesAlt(List<List<string>> groups, ref List<PolygonTicker> assets)
        {
            foreach (var group in groups)
            {
                var response = _client.GetBarSetAsync(group, TimeFrame.Day, 2).Result;
                foreach (var r in response)
                {
                    if (r.Value.Count() != 2)
                    {
                        assets.Remove(assets.First(f => f.ticker == r.Key));
                        continue;
                    }
                    bool isLowVolume = r.Value.First().Volume < MinLastVolume;
                    bool isLosingValue = r.Value.Last().Close < r.Value.First().Close;
                    bool isTooExpensive = r.Value.First().Close > MaxWatchPrice;
                    bool isTooCheap = r.Value.Last().Close < MinWatchPrice;

                    if (isLowVolume || isLosingValue || isTooExpensive || isTooCheap)
                    {
                        assets.Remove(assets.First(f => f.ticker == r.Key));
                    }
                }
            }
        }










        private void FilterAssets(ref List<IAsset> assets)
        {
            assets = assets.Where(o => o.IsTradable
                    && o.Status == AssetStatus.Active
                    && o.Class == AssetClass.UsEquity
                    && (o.Exchange == Exchange.Nasdaq || o.Exchange == Exchange.Nyse))
                .ToList();
        }

        private List<List<string>> ConvertListToGroupsOfSymbols(List<IAsset> assets)
        {
            var AssetsGroups = new List<List<string>>();
            int count = 0;
            for (int i = 0; i < assets.Count; i += GroupSize)
            {
                var temp = assets.GetRange(i, Math.Min(GroupSize, assets.Count - i));
                AssetsGroups.Add(new List<string>());
                foreach (var item in temp)
                {
                    AssetsGroups[count].Add(item.Symbol);
                }
                count++;
            }
            return AssetsGroups;
        }

        private void NarrowDownAssetsByRules(List<List<string>> groups, ref List<IAsset> assets)
        {
            foreach (var group in groups)
            {
                var response = _client.GetBarSetAsync(group, TimeFrame.Day, 2).Result;
                foreach (var r in response)
                {
                    bool isBadCount = r.Value.Count() != 2;
                    bool isLowVolume = r.Value.First().Volume < MinLastVolume;
                    bool isLosingValue = r.Value.Last().Close < r.Value.First().Close;
                    bool isTooExpensive = r.Value.First().Close > MaxWatchPrice;
                    bool isTooCheap = r.Value.First().Close < MinWatchPrice;
                    if (isBadCount || isLowVolume || isLosingValue || isTooExpensive || isTooCheap)
                    {
                        assets.Remove(assets.First(f => f.Symbol == r.Key));
                    }
                }
            }
        }

        private List<PolygonTicker> CallPolygonApi()
        {
            var client = new RestSharp.RestClient("https://api.polygon.io/v2/reference/tickers");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Host", "api.polygon.io");
            request.AddHeader("Accept", "*/*");
            request.AddParameter("type", "cs");
            request.AddParameter("market", "stocks");
            request.AddParameter("locale", "us");
            request.AddParameter("active", "true");
            request.AddParameter("perpage", 1000);
            request.AddQueryParameter("apiKey", "");
            IRestResponse response = client.Execute(request);

            var content = JsonConvert.DeserializeObject<PolygonStockAssets>(response.Content);
            return content.tickers;
        }
    }

    public class PolygonCodes
    {
        public string figiuid { get; set; }
        public string scfigi { get; set; }
        public string cfigi { get; set; }
        public string figi { get; set; }
        public string cik { get; set; }
    }

    public class PolygonTicker
    {
        public string ticker { get; set; }
        public string name { get; set; }
        public string market { get; set; }
        public string locale { get; set; }
        public string type { get; set; }
        public string currency { get; set; }
        public bool active { get; set; }
        public string primaryExch { get; set; }
        public string updated { get; set; }
        public PolygonCodes codes { get; set; }
        public string url { get; set; }
    }

    public class PolygonStockAssets
    {
        public int page { get; set; }
        public int perPage { get; set; }
        public int count { get; set; }
        public string status { get; set; }
        public List<PolygonTicker> tickers { get; set; }
    }
}
