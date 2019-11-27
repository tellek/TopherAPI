using Alpaca.Markets;
using StocksMonitor.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using static Contracts.AppSettings;

namespace StocksMonitor.Processes
{
    public partial class AssetCollection
    {
        private RestClient client;
        // See AssetCollection_Properties.cs for access to all associated properties.

        public AssetCollection()
        {
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        /// <summary>
        /// Return assets based on configured rules as a list of IAsset.
        /// </summary>
        /// <returns>list of IAsset</returns>
        public List<IAsset> GetFilteredAssetsAsList()
        {
            var assets = GetAssets();
            FilterAssets(ref assets, ConvertListToGroupsOfSymbols(assets));

            return assets;
        }

        /// <summary>
        /// Return assets based on configured rules as a Dictionary of string and new AssetData.
        /// </summary>
        /// <returns>Dictionary of string and new AssetData</returns>
        public Dictionary<string, AssetData> GetFilteredAssetsAsDictionary()
        {
            var assets = GetAssets();
            FilterAssets(ref assets, ConvertListToGroupsOfSymbols(assets));

            var results = new Dictionary<string, AssetData>();
            foreach (var asset in assets)
            {
                results.Add(asset.Symbol, new AssetData());
            }

            return results;
        }

        /// <summary>
        /// Return assets based on configured rules as a ConcurrentDictionary of string and new AssetData.
        /// </summary>
        /// <returns>ConcurrentDictionary of string and new AssetData</returns>
        public ConcurrentDictionary<string, AssetData> GetFilteredAssetsAsConcurrentDictionary()
        {
            var assets = GetAssets();
            FilterAssets(ref assets, ConvertListToGroupsOfSymbols(assets));

            var results = new ConcurrentDictionary<string, AssetData>();
            foreach (var asset in assets)
            {
                results.TryAdd(asset.Symbol, new AssetData());
            }

            return results;
        }

        private void FilterAssets(ref List<IAsset> assets, List<List<string>> groups)
        {
            foreach (var group in groups)
            {
                var response = client.GetBarSetAsync(group,
                    timeFrame: TimeBasis,
                    limit: Limit,
                    timeFrom: TimeFrom,
                    timeInto: TimeTo).GetAwaiter().GetResult();

                foreach (var r in response)
                {
                    bool isBadCount = r.Value.Count() < GoodAmount;
                    bool isLowVolume = r.Value.First().Volume < MinLastVolume;
                    bool isLosingValue = r.Value.Last().Close < r.Value.First().Close;
                    bool isTooExpensive = r.Value.Last().Close > MaxPrice;
                    bool isTooCheap = r.Value.Last().Close < MinPrice;
                    if ((FilterIfBadCount && isBadCount) 
                        || (FilterIfLowVolume && isLowVolume)
                        || (FilterIfLosingValue && isLosingValue) 
                        || (FilterIfTooExpensive && isTooExpensive)
                        || (FilterIfTooCheap && isTooCheap))
                    {
                        assets.Remove(assets.First(f => f.Symbol == r.Key));
                    }
                }
            }
        }

        private List<List<string>> ConvertListToGroupsOfSymbols(List<IAsset> assets, int groupSize = 100)
        {
            var AssetsGroups = new List<List<string>>();
            int count = 0;
            for (int i = 0; i < assets.Count; i += groupSize)
            {
                var temp = assets.GetRange(i, Math.Min(groupSize, assets.Count - i));
                AssetsGroups.Add(new List<string>());
                foreach (var item in temp)
                {
                    AssetsGroups[count].Add(item.Symbol);
                }
                count++;
            }
            return AssetsGroups;
        }

        private List<IAsset> GetAssets()
        {
            var assets = client.ListAssetsAsync().GetAwaiter().GetResult().ToList();

            assets = assets.Where(o => o.IsTradable
                    && o.Status == AssetStatus.Active
                    && o.Class == AssetClass.UsEquity
                    && (o.Exchange == Exchange.Nasdaq || o.Exchange == Exchange.Nyse))
                .ToList();

            return assets;
        }
    }
}
