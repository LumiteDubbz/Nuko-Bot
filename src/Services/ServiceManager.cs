using Discord.Commands;
using Discord.WebSocket;
using NukoBot.Common;
using NukoBot.Events;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using NukoBot.Database.Models;
using NukoBot.Database.Repositories;

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
            var runCounterCollection = database.GetCollection<Run>("runCount");
            var runCounter = runCounterCollection.AsQueryable();
            var isEmpty = !runCounter.Any();

            int runNumber = 1;

            if (!isEmpty)
            {
                var query = runCounter.OrderByDescending(x => x.RunNumber).Take(1);

                runNumber = query.First().RunNumber + 1;
            }

            var count = new Run()
            {
                TimeCommenced = DateTime.Now,
                RunNumber = runNumber
            };

            runCounterCollection.InsertOne(count);

            var services = new ServiceCollection()
                .AddSingleton<Logger>()
                .AddSingleton<Text>()
                .AddSingleton<MessageReceived>()
                .AddSingleton<Ready>()
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddSingleton(_credentials)
                .AddSingleton(database.GetCollection<Guild>("guilds"))
                .AddSingleton(database.GetCollection<Mute>("mutes"))
                .AddSingleton(database.GetCollection<User>("users"))
                .AddSingleton<GuildRepository>()
                .AddSingleton<MuteRepository>()
                .AddSingleton<UserRepository>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public IMongoDatabase ConfigureDatabase()
        {
            var mongoClient = new MongoClient(_credentials.DatabaseConnectionString);

            return mongoClient.GetDatabase(_credentials.DatabaseName);
        }
    }
}
