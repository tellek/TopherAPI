using DiscordBot;
using Microsoft.Extensions.DependencyInjection;
using StocksMonitor.Processes;
using StocksMonitor.ScheduledJobs;
using StocksMonitor.Tasks;
using System;
using System.Collections.Generic;
using System.Text;

namespace StocksMonitor
{
    public static class StocksInjection
    {
        public static void ConfigureStocksMonitorClasses(IServiceCollection services)
        {
            services.AddHostedService<SocksMonitorHost>();
            ConfigureTasks(services);
            ConfigureJobs(services);
            ConfigureProcesses(services);
        }

        private static void ConfigureTasks(IServiceCollection services)
        {
            services.AddSingleton<HealthiestStocks>();
            services.AddSingleton<GetAggregateData>();
            services.AddSingleton<ManageAssets>();
        }

        private static void ConfigureJobs(IServiceCollection services)
        {
            services.AddSingleton<ImmediateJob>();
            services.AddSingleton<WatchSellJob>();
            services.AddTransient<StopLossJob>();
            services.AddTransient<RegularJob>();
            services.AddTransient<MorningJob>();
            services.AddTransient<AfternoonJob>();
            services.AddTransient<NightlyJob>();
        }

        private static void ConfigureProcesses(IServiceCollection services)
        {
            services.AddTransient<GetFilteredStocks>();
        }
    }
    
}
