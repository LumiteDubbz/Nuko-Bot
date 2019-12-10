using Discord;
using MongoDB.Driver;
using NukoBot.Services;

namespace NukoBot.Common.Structures
{
    public class Globals
    {
        public Globals(IDiscordClient client, IGuild guild, IMongoDatabase database, Text text)
        {
            Client = client;
            Guild = guild;
            Database = database;
            Text = text;
        }

        public IDiscordClient Client { get; }

        public IGuild Guild { get; }

        public IMongoDatabase Database { get; }

        public Text Text { get; }
    }
}
