using MongoDB.Bson;

namespace NukoBot.Database.Models
{
    public partial class Guild : Model
    {
        public Guild(ulong guildId)
        {
            GuildId = guildId;
        }

        public ulong GuildId
        {
            get;
            set;
        }

        public BsonDocument ModRoles
        {
            get;
            set;
        } = new BsonDocument();

        public ulong MutedRoleId
        {
            get;
            set;
        }

        public ulong ModLogChannelId
        {
            get;
            set;
        }

        public ulong ScreenshotChannelId
        {
            get;
            set;
        }

        public ulong NewUserRole
        {
            get;
            set;
        }

        public ulong TopThreeRole
        {
            get;
            set;
        }

        public string WelcomeMessage
        {
            get;
            set;
        } = string.Empty;

        public int CaseNumber
        {
            get;
            set;
        } = 1;

        public int Points
        {
            get;
            set;
        } = 0;

        public double PointMultiplier
        {
            get;
            set;
        } = 1;
    }
}