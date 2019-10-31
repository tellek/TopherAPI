using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TopherAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .UseEnvironment((Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "local").ToLowerInvariant())
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseSerilog((context, config) =>
                   {
                       config.Filter.ByExcluding(lo => lo.RenderMessage().Contains("AuthenticationScheme: "));
                       config.Enrich.FromLogContext();
                   })
                .ConfigureAppConfiguration((hostingContext, config) =>
                   {
                       config.AddCommandLine(args);
                       config.AddJsonFile("appsettings.json").AddJsonFile("appsettings.json", optional: false);
                   })
                .ConfigureKestrel((context, options) =>
                {
                    // Set properties and call methods on options
                })
                .Build()
                .Run();
        }

    }
}
