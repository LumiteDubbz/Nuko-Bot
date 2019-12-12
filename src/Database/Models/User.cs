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

        // Numbers

        public ulong UserId { get; set; }

        public ulong GuildId { get; set; }

        public int Points { get; set; } = 0;

        // Lists

        private List<Milestone> milestones = new List<Milestone>();

        public List<Milestone> Milestones
        {
            get { return milestones; } set { milestones = value; }
        }

        private List<Warning> warnings = new List<Warning>();

        public List<Warning> Warnings
        {
            get { return warnings; } set { warnings = value; }
        }

        // Bools

        public bool HasBeenMuted { get; set; } = false;

        public bool HasBeenKicked { get; set; } = false;

        public bool HasBeenBanned { get; set; } = false;
    }
}