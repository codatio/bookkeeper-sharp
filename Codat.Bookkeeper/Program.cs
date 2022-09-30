using Codat.Bookkeeper.DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;

namespace Codat.Bookkeeper
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();

            try
            {
                var host = CreateHostBuilder(args).Build();

                // Apply migrations on startup
                using (var scope = host.Services.CreateScope())
                {
                    using var context = scope.ServiceProvider.GetRequiredService<BookkeeperDbContext>();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                host.Run();
            }
            catch(Exception ex)
            {
                logger.Error(ex, "Fatal exception");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
    }
}
