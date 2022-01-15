using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DictionaryBackWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(config)
                .CreateLogger();

            try
            {
                Log.Information("Application Starting.");
                CreateHostBuilder(args).Build().Run();
            }
            catch (System.Exception ex)
            {
                Log.Fatal(ex, "fatal error");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddMassTransit(busConfig =>
                    {
                        busConfig.AddConsumer<WordMessageConsumer>();
                        busConfig.AddConsumer<RepetitionResultConsumer>();

                        // todo config
                        busConfig.UsingRabbitMq((context, rabbitCfg) =>
                        {
                            rabbitCfg.Host("amqp://host.docker.internal", h =>
                            {
                                h.Username("guest");
                                h.Password("guest");
                            });

                            rabbitCfg.ConfigureEndpoints(context);
                        });

                    });
                    services.AddMassTransitHostedService();

                    services.AddHostedService<Worker>();
                });
    }
}
