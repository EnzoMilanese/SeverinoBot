using DSharpPlus;
using DSharpPlus.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SeverinoBot
{
    public class Program
    {
        public static IConfigurationRoot configuration;

        public static async Task Main(string[] args)
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            var discordBotKey = configuration["discordBotKey"];
            var defaultChannelId = configuration["defaultChannelId"];

            var discordClient = new DiscordClient(new DiscordConfiguration
            {
                Token = discordBotKey,
                TokenType = TokenType.Bot,
                UseInternalLogHandler = true,
                LogLevel = LogLevel.Debug
            });

            DiscordChannel defaultChannel = null;
            if (ulong.TryParse(defaultChannelId, out var channelId))
                defaultChannel = await discordClient.GetChannelAsync(channelId);
            if (defaultChannel != null)
                await defaultChannel.SendMessageAsync("Salve salve bolsonaroi");

            discordClient.MessageCreated += MessageController.OnMessageCreated;

            await discordClient.ConnectAsync();
            await Task.Delay(-1);
        }

        private static void ConfigureServices(IServiceCollection serviceCollection)
        {
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();

            serviceCollection.AddSingleton<IConfigurationRoot>(configuration);
        }
    }
}