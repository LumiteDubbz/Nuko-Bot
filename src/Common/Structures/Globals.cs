using Discord;
using MongoDB.Driver;
using NukoBot.Database.Models;
using NukoBot.Services;

namespace NukoBot.Common.Structures
{
    public class Globals
    {
        public Globals(IDiscordClient client, IGuild guild, Text text, IMongoCollection<User> users)
        {
            Client = client;
            Guild = guild;
            Users = users;
            Text = text;
        }

        public IDiscordClient Client { get; }

        public IGuild Guild { get; }

        public IMongoCollection<User> Users { get; }

        public Text Text { get; }
    }
}
