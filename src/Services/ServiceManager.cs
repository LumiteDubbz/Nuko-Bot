using Discord.Commands;
using Discord.WebSocket;
using NukoBot.Common;
using NukoBot.Events;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
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

            var services = new ServiceCollection()
                // Parameters
                .AddSingleton(_client)
                .AddSingleton(_commandService)
                .AddSingleton(_credentials)
                // DB Models
                .AddSingleton(database.GetCollection<Guild>("guilds"))
                .AddSingleton(database.GetCollection<Mute>("mutes"))
                .AddSingleton(database.GetCollection<Poll>("polls"))
                .AddSingleton(database.GetCollection<Run>("runs"))
                .AddSingleton(database.GetCollection<User>("users"))
                // DB Repositories
                .AddSingleton<GuildRepository>()
                .AddSingleton<MuteRepository>()
                .AddSingleton<PollRepository>()
                .AddSingleton<RunRepository>()
                .AddSingleton<UserRepository>()
                // Events
                .AddSingleton<MessageReceived>()
                .AddSingleton<Ready>()
                .AddSingleton<UserJoined>()
                // Services
                .AddSingleton<ModerationService>()
                .AddSingleton<Text>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public IMongoDatabase ConfigureDatabase()
        {
            var mongoClient = new MongoClient(_credentials.DatabaseConnectionString);

            return mongoClient.GetDatabase(_credentials.DatabaseName);
        }
    }
}
