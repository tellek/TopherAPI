using System;
using System.Threading;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using StocksMonitor.Processes;
using FluentScheduler;
using System.Diagnostics;
using Autofac;
using StocksMonitor.ScheduledJobs;
using DiscordBot;

namespace StocksMonitor
{
    public partial class SocksMonitorHost : IHostedService
    {
        private readonly ImmediateJob _immediateJob;
        private readonly WatchSellJob _watchSellJob;
        private readonly RegularJob _regularJob;
        private readonly MorningJob _morningJob;
        private readonly AfternoonJob _afternoonJob;
        private readonly NightlyJob _nightlyJob;

        public SocksMonitorHost(WatchSellJob constantJob, MorningJob morningJob,
                                AfternoonJob afternoonJob, NightlyJob nightlyJob,
                                RegularJob regularJob, ImmediateJob immediateJob,
                                WatchSellJob watchSellJob)
        {
            _immediateJob = immediateJob;
            _regularJob = regularJob;
            _watchSellJob = watchSellJob;
            _morningJob = morningJob;
            _afternoonJob = afternoonJob;
            _nightlyJob = nightlyJob;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            JobManager.Start();

            // Constant job that attempts to sell at a positive percentage for profit or prevent too much loss by selling early.
            Action<Schedule> watchSellSchedule = s => s.ToRunEvery(60).Seconds().Between(8, 30, 14, 57);
            JobManager.AddJob(_watchSellJob, watchSellSchedule);

            // Every hour
            //Action<Schedule> regularSchedule = s => s.ToRunEvery(15).Minutes().Between(8, 30, 15, 00);
            //JobManager.AddJob(_regularJob, regularSchedule);

            // Every night
            //Action<Schedule> nightlySchedule = s => s.ToRunEvery(1).Weekdays().At(2, 0);
            //JobManager.AddJob(_nightlyJob, nightlySchedule);

            // Every morning
            Action<Schedule> morningSchedule = s => s.ToRunEvery(1).Weekdays().At(10, 0);
            JobManager.AddJob(_morningJob, morningSchedule);

            // Every afternoon
            //Action<Schedule> afternoonSchedule = s => s.ToRunEvery(1).Weekdays().At(14, 58);
            //JobManager.AddJob(_afternoonJob, afternoonSchedule);

            // Testing something new
            Action<Schedule> immediateSchedule = s => s.ToRunOnceIn(1).Seconds();
            JobManager.AddJob(_immediateJob, immediateSchedule);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            JobManager.StopAndBlock();
            return Task.CompletedTask;
        }
    }

}
