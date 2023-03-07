using Alpaca.Markets;
using DiscordBot;
using Am = Alpaca.Markets;

namespace StocksMonitor.Tasks
{
    public class AccountTasks
    {
        private Am.RestClient client;
        private DiscordCommands discord;

        public AccountTasks(RestClient client, DiscordCommands discord)
        {
            this.discord = discord;
            this.client = client;
        }

        public void CancelOrders()
        {
            var orders = client.ListOrdersAsync().Result;
            foreach (var o in orders)
            {
                var wasSuccess = client.DeleteOrderAsync(o.OrderId).GetAwaiter().GetResult();
                if (wasSuccess)
                {
                    discord.Log($"Order {o.OrderId} for {o.Symbol} cancelled.");
                }
                else
                {
                    discord.Log($"Failed to cancel order {o.OrderId} for {o.Symbol}.");
                }
            }
        }
    }
}
