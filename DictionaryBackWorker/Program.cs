using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using DictionaryBackWorker.MQ;
using DictionaryBack.Messages;
using System;
using GreenPipes;

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

                    services.AddOptions<RabbitSettings>().BindConfiguration(RabbitSettings.SectionName);

                    RabbitSettings opts = hostContext.Configuration
                    .GetSection(RabbitSettings.SectionName)
                    .Get<RabbitSettings>();


                    services.AddMassTransit(busConfig =>
                    {
                        busConfig.AddConsumer<WordMessageConsumer>();
                        busConfig.AddConsumer<RepetitionResultConsumer>();

                        busConfig.UsingRabbitMq((context, rabbitCfg) =>
                        {
                            rabbitCfg.Host(opts.Host, h =>
                            {
                                h.Username(opts.Username);
                                h.Password(opts.Password);
                            });

                            rabbitCfg.ReceiveEndpoint(queueName: "RepetitionResult", endpointConfigurator =>
                            {
                                endpointConfigurator.Bind(exchangeName: "DictionaryBack.Messages:RepetitionEndedMessage");
                                endpointConfigurator.Bind<RepetitionEndedMessage>();
                                endpointConfigurator.UseMessageRetry(m => m.Interval(3, TimeSpan.FromSeconds(2)));
                            });


                            rabbitCfg.ReceiveEndpoint(queueName: "WordMessage", endpointConfigurator =>
                            {
                                endpointConfigurator.Bind(exchangeName: "DictionaryBack.Messages:WordMessage");
                                endpointConfigurator.Bind<WordMessage>();
                                endpointConfigurator.UseMessageRetry(m => m.Interval(3, TimeSpan.FromSeconds(2)));
                            });
                        
                        });
                    });

                    services.AddMassTransitHostedService();

                    services.AddHostedService<Worker>();
                });
    }
}
