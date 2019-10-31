using Alpaca.Markets;
using Contracts.StocksMonitor;
using System;
using System.Collections.Generic;
using System.Linq;
using static Contracts.AppSettings;

namespace StocksMonitor.Tasks
{
    public class GetAggregateData
    {
        private RestClient client;

        public GetAggregateData()
        {
            client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        public Dictionary<string, AssetAgg> Execute(List<IAsset> assets, int hoursBack)
        {
            var group = new List<string>();
            foreach (var w in assets) { group.Add(w.Symbol); }

            // Downloading 5 Minute Data for assets...");
            //var response = client.GetBarSetAsync(group, TimeFrame.FiveMinutes, 100).GetAwaiter().GetResult();
            //// Should try the from and to dateTimes.........
            //hoursBack = DateTime.Now.DayOfWeek == DayOfWeek.Monday ? hoursBack + 48 : hoursBack;

            //var results = new Dictionary<string, AssetAgg>();

            //foreach (var r in response)
            //{
            //    results.Add(r.Key, new AssetAgg {
            //        Score = 0,
            //        Agg = r.Value.Where(x =>
            //                        x.Time <= DateTime.Now
            //                        && x.Time >= DateTime.Now.AddHours(-hoursBack)).ToList()
            //    });
            //}

            var response = client.GetBarSetAsync(group, TimeFrame.FiveMinutes, null, timeFrom: DateTime.Now.AddHours(-hoursBack), timeInto: DateTime.Now).GetAwaiter().GetResult();

            var results = new Dictionary<string, AssetAgg>();
            foreach (var r in response)
            {
                if (r.Value.Count() < 10) continue;
                results.Add(r.Key, new AssetAgg {
                    Score = 0,
                    Agg = r.Value.ToList()
                });
            }

            return results;
        }
    }
}
