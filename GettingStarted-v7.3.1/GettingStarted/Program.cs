using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using GettingStarted.Diagnostic;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Net.Http.Headers;
using Serilog;

namespace GettingStarted
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                //var subscription = DiagnosticListener.AllListeners.Subscribe(new DiagnosticObserver( new MassTransitDiagnosticObserver()));

                // web builder
                var builder = CreateWebBuilder(args);
                var app = builder.Build();
                Configure(app, builder.Environment);
                await app.RunAsync();
                
                // host builder
                //CreateHostBuilder(args).Build().Run();

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
#if DEBUG
                await Console.Error.WriteLineAsync("Host terminated unexpectedly: " + ex.ToString());
#endif
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static void Configure(object app, object environment)
        {
        }

        public static WebApplicationBuilder CreateWebBuilder(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddSingleton<MassTransitDiagnosticObserver>();
            builder.Services.AddHostedService<DiagnosticObserver>();
            
            builder.Services.AddMassTransit(x =>
            {
                x.AddConsumer<MessageConsumer>();

                x.UsingRabbitMq((context,cfg) =>
                {
                    cfg.ConfigureEndpoints(context);
                });
            });

            builder.Services.AddHostedService<Worker>();

            return builder;
        }
        
        
        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<MassTransitDiagnosticObserver>();
                    services.AddHostedService<DiagnosticObserver>();
                    
                    services.AddMassTransit(x =>
                    {
                        x.AddConsumer<MessageConsumer>();

                        x.UsingRabbitMq((context,cfg) =>
                        {
                            cfg.ConfigureEndpoints(context);
                        });
                    });

                    services.AddHostedService<Worker>();
                    
                    
                });

            return host;
        }

    }
}
