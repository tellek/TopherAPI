using Alpaca.Markets;
using Contracts.StocksMonitor;
using DiscordBot;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using static Contracts.AppSettings;

namespace StocksMonitor.Tasks
{
    public class ManageAssets
    {
        private readonly DiscordCommands discord;

        private int MaxOwnedStocks = 4;
        private long PurchaseAmount = 50;

        public ManageAssets(DiscordCommands discord)
        {
            this.discord = discord;
        }

        public void BuyStocks(RestClient client, List<IPosition> positions)
        {
            var newStocksList = Memory.RememberedStocks.Where(x => Memory.JustSoldStocks.Contains(x.Key) == false);
            foreach (var rStk in newStocksList.Take(MaxOwnedStocks))
            {
                if (positions.Any(x => x.Symbol == rStk.Key) || Memory.JustSoldStocks.Contains(rStk.Key))
                    continue;

                var result = client.PostOrderAsync(rStk.Key, PurchaseAmount, OrderSide.Buy, OrderType.Market, TimeInForce.Day).GetAwaiter().GetResult();
                discord.Say($"Purchased {PurchaseAmount} shares of {rStk.Key}.");
            }
            Memory.JustSoldStocks.Clear();
        }

        public void SellStocks(RestClient client, List<IPosition> positions)
        {
            if (positions == null || positions.Count <= 0)
            {
                discord.Say($"There are not owned stocks to sell.");
                return;
            }

            var results = client.DeleteAllPositionsAsync().GetAwaiter().GetResult();
            Thread.Sleep(100);

            foreach (var r in results)
            {
                if (r.IsSuccess)
                {
                    discord.Say($"Liquidated all shares of {r.Symbol}.");
                    Memory.JustSoldStocks.Add(r.Symbol);
                }
                else
                {
                    discord.Say($"Failed to liquidate all shares of {r.Symbol}.");
                }
                
            }
        }

        public void CancelOrders(RestClient client)
        {
            var sb = new StringBuilder();
            var canceled = client.DeleteAllOrdersAsync().GetAwaiter().GetResult();
            foreach (var can in canceled)
            {
                if (can.IsSuccess) sb.AppendLine($"Canceled order {can.OrderId}.");
                else sb.AppendLine($"Failed to cancel {can.OrderId}.");
            }
            discord.Say(sb.ToString());
        }

        public List<IPosition> GetPositions(RestClient client)
        {
            var positions = client.ListPositionsAsync().GetAwaiter().GetResult().ToList();
            return positions;
        }
    }
}
