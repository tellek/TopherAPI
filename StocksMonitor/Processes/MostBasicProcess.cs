using Alpaca.Markets;
using StocksMonitor.Tasks;
using static Contracts.AppSettings;

namespace StocksMonitor.Processes
{
    public class MostBasicProcess
    {
        private readonly RestClient _client;

        public MostBasicProcess()
        {
            _client = new RestClient(Settings.AlpacaKeyId, Settings.AlpacaSecret, Settings.AlpacaApiUrl);
        }

        public void Execute()
        {
            
            

            // In the morning remove the stoplosses and add stoploss for average price paid.
            // At afternoon sell everything and buy new stocks.

            // The actual money maker.
            // Every 10 seconds check owned stocks and get the current price of each. 
            // Once the th gain reaches 2%, limit sell for 2%.
            // Limit sell at -0.25% current price when at or lower than original purchase price.
            // Sell everything and buy all new at end of day.
        }
    }
}
