using MongoDB.Bson;
using NukoBot.Common;

namespace NukoBot.Database.Models
{
    public partial class Guild : Model
    {
        public Guild(ulong guildId) {
            GuildId = guildId;
        }

        public ulong GuildId { get; set; }

        // BsonDocuments

        public BsonDocument ModRoles { get; set; } = new BsonDocument();

        public BsonDocument RankRoles { get; set; } = new BsonDocument();

        public BsonDocument CustomCommands { get; set; } = new BsonDocument();

        // Strings

        public string WelcomeMessage { get; set; } = string.Empty;

        public string Prefix { get; set; } = Configuration.Prefix;

        public string WarnPunishment { get; set; } = Configuration.DefaultWarnPunishment;

        // Numbers

        public int CaseNumber { get; set; } = 1;

        public int Points { get; set; } = 0;

        public double PointMultiplier { get; set; } = 1;

        public ulong NewUserRole { get; set; }

        public ulong TopThreeRole { get; set; }

        public ulong MutedRoleId { get; set; }

        public ulong ModLogChannelId { get; set; }

        public ulong ScreenshotChannelId { get; set; }

        public int DaysUntilWarningExpires { get; set; } = Configuration.DaysUntilWarningExpires;

        public int MaxmimumWarningsBeforeAction { get; set; } = Configuration.MaximumWarningsBeforeAction;

        public double WarnMuteLength { get; set; } = Configuration.WarnMuteLength;

        // Arrays

        public string[] DisabledCommands { get; set; } = new string[] { };

        public ulong[] IgnoredChannels { get; set; } = new ulong[] { };

        // Bools

        public bool OverhaulEnabled { get; set; } = false;

        public bool LockedDown { get; set; } = false;

        public bool LogMessageDeletions { get; set; } = false;

        public bool LogMessageEdits { get; set; } = false;
    }
}