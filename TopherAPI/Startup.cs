﻿using System;
using Contracts;
using DiscordBot;
using DiscordBot.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StocksMonitor;
using TopherAPI.Services;
using static Contracts.AppSettings;

namespace TopherAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            var variables = Environment.GetEnvironmentVariable("TopherApiSettings", EnvironmentVariableTarget.User);
            Settings = JsonConvert.DeserializeObject<Values>(variables);
        }

        public async void ConfigureServices(IServiceCollection services)
        {
            services.ConfigureDiscord();
            services.ConfigureMvc();
            services.ConfigureSwagger();
            services.ConfigureDI();
            

            var provider = services.BuildServiceProvider();     // Build the service provider
            await provider.GetRequiredService<DiscordBotMain>().StartAsync();
            provider.GetRequiredService<LoggingHandler>();      // Start the logging service
            provider.GetRequiredService<CommandHandler>(); 		// Start the command handler service
            
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.ConfigureMvc();
            app.ConfigureSwagger();
        }
    }

    

}
