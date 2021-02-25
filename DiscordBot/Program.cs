using DiscordBot.Core.Extensions;
using DiscordBot.Handlers;
using DiscordBot.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace DiscordBot
{
    class Program
    {
        public static IConfigurationRoot Configuration;

        static void Main(string[] args)
        {
            BuildConfiguration();

            var services = ConfigureServices();

            services.GetService<IDiscordClient>().Run();

            while (true)
            {
                Console.ReadKey(true);
            }
        }

        static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            services.AddLogging(logging =>
            {
                logging
                    .AddConsole()
                    .AddConfiguration(Configuration.GetSection("Logging"));
            }).AddTransient<IDiscordClient, DiscordClient>();

            Assembly.GetEntryAssembly().GetTypesAssignableFrom<IHandler>().ForEach((t) =>
            {
                services.AddScoped(typeof(IHandler), t);
            });

            var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger<Program>>();
            services.AddSingleton(typeof(ILogger), logger);

            return services.BuildServiceProvider();
        }

        static void BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(AppContext.BaseDirectory))
                .AddJsonFile("appsettings.json", optional: false);

            Configuration = builder.Build();
        }
    }
}
