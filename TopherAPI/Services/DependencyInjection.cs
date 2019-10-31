using Microsoft.Extensions.DependencyInjection;
using StocksMonitor;
using DiscordBot;
using Contracts.StocksMonitor;
using StocksMonitor.Engines;

namespace TopherAPI.Services
{
    public static class DependencyInjection
    {
        public static void ConfigureDI(this IServiceCollection services)
        {
            StocksInjection.ConfigureStocksMonitorClasses(services);
            ConfigureStockMarketMethods(services);
            services.AddSingleton<DiscordCommands>();
        }

        private static void ConfigureStockMarketMethods(IServiceCollection services)
        {
            services.AddSingleton<IMarketInformation, GetMarketInformation>();
        }
    }
}
