using NukoBot.Common.Structures;

namespace NukoBot.Common
{
    public static class Maps
    {
        public static Map Easy01Map = new Map()
        {
            Name = "Easy 01",
            Difficulty = 1,
            Multiplier = 1,
            Milestones = Milestones.Easy
        };

        public static Map Normal01Map = new Map()
        {
            Name = "Normal 01",
            Difficulty = 2,
            Multiplier = 1.15,
            Milestones = Milestones.Normal
        };

        public static Map Normal02Map = new Map()
        {
            Name = "Normal 02",
            Difficulty = 2,
            Multiplier = 1.15,
            Milestones = Milestones.Normal
        };

        public static Map Hard01Map = new Map()
        {
            Name = "Hard 01",
            Difficulty = 3,
            Multiplier = 1.3,
            Milestones = Milestones.Hard
        };
    }
}
