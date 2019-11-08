using System.Collections.Generic;

namespace NukoBot.Common.Structures
{
    public sealed class Map
    {
        public string Name { get; set; }

        public int Difficulty { get; set; }

        public double Multiplier { get; set; }

        private List<Milestone> milestones = new List<Milestone>();

        public List<Milestone> Milestones { get { return milestones; } set { milestones = value; } }
    }
}