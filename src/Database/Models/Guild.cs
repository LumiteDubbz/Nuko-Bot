using Discord.Commands;
using MongoDB.Bson;
using System.Collections.Generic;

namespace NukoBot.Database.Models
{
    public partial class Guild : Model
    {
        public Guild(ulong guildId) {
            GuildId = guildId;
        }

        public ulong GuildId { get; set; }

        // Roles

        public BsonDocument ModRoles { get; set; } = new BsonDocument();

        public BsonDocument RankRoles { get; set; } = new BsonDocument();

        public ulong NewUserRole { get; set; }

        public ulong TopThreeRole { get; set; }

        public ulong MutedRoleId { get; set; }

        // Channels

        public ulong ModLogChannelId { get; set; }

        public ulong ScreenshotChannelId { get; set; }

        // Messages

        public string WelcomeMessage { get; set; } = string.Empty;

        // Numbers

        public int CaseNumber { get; set; } = 1;

        public int Points { get; set; } = 0;

        public double PointMultiplier { get; set; } = 1;

        // Misc.

        public string[] DisabledCommands { get; set; } = new string[] { };

        // Bools

        public bool OverhaulEnabled { get; set; } = true;
    }
}