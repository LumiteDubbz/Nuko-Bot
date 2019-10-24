using Discord.Commands;
using Discord.WebSocket;
using NukoBot.Common;
using NukoBot.Events;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System;

namespace NukoBot.Services
{
    public sealed class ServiceManager
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly Credentials _credentials;

        public IServiceProvider ServiceProvider { get; }

        public ServiceManager(DiscordSocketClient client, CommandService commandService, Credentials credentials)
        {
            _client = client;
            _commandService = commandService;
            _credentials = credentials;

            var database = ConfigureDatabase();

            var services = new ServiceCollection()
                .AddSingleton<Logger>()
                .AddSingleton<Text>()
                .AddSingleton<MessageReceived>()
                .AddSingleton<Ready>()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddSingleton(_credentials)
                .AddSingleton(database);

            ServiceProvider = services.BuildServiceProvider();
        }

        public IMongoDatabase ConfigureDatabase()
        {
            var mongoClient = new MongoClient(_credentials.DatabaseConnectionString);

            return mongoClient.GetDatabase(_credentials.DatabaseName);
        }
    }
}
