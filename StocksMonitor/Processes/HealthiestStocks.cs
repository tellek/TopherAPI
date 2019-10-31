using Alpaca.Markets;
using System;
using System.Collections.Generic;
using System.Linq;
using static Contracts.AppSettings;

namespace StocksMonitor.Processes
{
    public class HealthiestStocks
    {
        private const int GroupSize = 100;
        private const int MinumumAggregates = 50;
        private const int MinLastVolume = 500000;
        private const decimal MaxWatchPrice = 10.00m;
        private const decimal MinWatchPrice = 5.00m;
        private const int MaxPrimaries = 9;
        private RestClient client;

        public HealthiestStocks()
        {
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        public Dictionary<string, (decimal, ILastTrade)> FindThem()
        {
            var assets = client.ListAssetsAsync().Result.ToList();
            FilterAssets(ref assets);

            var groups = ConvertListToGroupsOfSymbols(assets);
            NarrowDownAssetsByRules(groups, ref assets);

            var aggregate = GetRecentTradeData(assets);
            var scores = ScoreTrendsOnTrades(aggregate);

            FilterDownToPrimaries(ref scores);
            var result = CreateFinalMarketDataObject(scores);

            return result;
        }

        private Dictionary<string,(decimal, ILastTrade)> CreateFinalMarketDataObject(Dictionary<string, decimal> scores)
        {
            var result = new Dictionary<string, (decimal, ILastTrade)>();
            foreach (var s in scores)
            {
                var temp = client.GetLastTradeAsync(s.Key).Result;
                result.Add(s.Key, (Math.Round(s.Value, 2), temp));
            }

            return result;
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
                var response = client.GetBarSetAsync(group, TimeFrame.Day, 2).Result;
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

        private Dictionary<string, List<IAgg>> GetRecentTradeData(List<IAsset> assets)
        {
            var group = new List<string>();
            foreach (var w in assets) { group.Add(w.Symbol); }

            // Downloading 5 Minute Data for assets...");
            var response = client.GetBarSetAsync(group, TimeFrame.FiveMinutes, 200).Result;

            int daysBack = DateTime.Now.DayOfWeek == DayOfWeek.Monday ? 3 : 1;
            var result = new Dictionary<string, List<IAgg>>();

            foreach (var r in response)
            {
                result.Add(r.Key, r.Value.Where(x =>
                                    x.Time.Day == DateTime.Now.Day
                                    || x.Time.Day == DateTime.Now.Day - daysBack).ToList());

                if (result[r.Key].Count < MinumumAggregates)
                {
                    result.Remove(r.Key);
                }
            }
            return result;
        }

        public Dictionary<string, decimal> ScoreTrendsOnTrades(Dictionary<string, List<IAgg>> data)
        {
            var scores = new Dictionary<string, decimal>();
            foreach (var w in data)
            {
                decimal score = 0;
                decimal firstOpen = w.Value.First().Open;
                decimal lastClose = w.Value.Last().Close;
                decimal expectedDiff = (lastClose - firstOpen) / w.Value.Count();
                decimal lastPrice = 0;

                if (lastClose > firstOpen)
                {
                    foreach (var rtd in w.Value)
                    {
                        decimal fullDiff = rtd.Close - firstOpen;
                        score = AdjustScore(fullDiff, score);

                        decimal thisDiff = rtd.Close - lastPrice;
                        score = AdjustScore(thisDiff, score);

                        decimal actualDiiff = expectedDiff - thisDiff;
                        score = AdjustScore(actualDiiff, score);

                        lastPrice = rtd.Close;
                    }
                }

                scores.Add(w.Key, score);
            }
            return scores;
        }

        private decimal AdjustScore(decimal value, decimal score)
        {
            (Maths value_R, decimal value_V) = IsPositive(value);
            switch (value_R)
            {
                case Maths.Equal:
                    return score;
                case Maths.Positive:
                    return score += value_V;
                case Maths.Negative:
                    return score -= value_V;
                default:
                    return score;
            }
        }

        private (Maths, decimal) IsPositive(decimal value)
        {
            if (value > 0)
            {
                //Positive
                return (Maths.Positive, value);
            }
            else if (value < 0)
            {
                //Negative
                var newValue = value * -1;
                return (Maths.Negative, newValue);
            }
            else
            {
                //Equal
                return (Maths.Equal, value);
            }
        }

        private void FilterDownToPrimaries(ref Dictionary<string, decimal> scores)
        {
            var primaries = new Dictionary<string, decimal>();
            var temp = scores.OrderByDescending(i => i.Value);

            for (int i = 0; i < MaxPrimaries; i++)
            {
                primaries.Add(temp.ElementAt(i).Key, temp.ElementAt(i).Value);
            }

            scores = primaries;
        }
    }

    public enum Maths
    {
        Equal,
        Positive,
        Negative
    }
}
