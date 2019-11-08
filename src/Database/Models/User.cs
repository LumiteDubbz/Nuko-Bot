using NukoBot.Common.Structures;
using System.Collections.Generic;

namespace NukoBot.Database.Models
{
    public partial class User : Model
    {
        public User(ulong userId, ulong guildId)
        {
            UserId = userId;
            GuildId = guildId;
        }

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public int Points { get; set; }

        private List<Milestone> milestones = new List<Milestone>();

        public List<Milestone> Milestones { get { return milestones; } set { milestones = value; } }
    }
}