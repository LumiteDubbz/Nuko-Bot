using Discord;
using System;

namespace NukoBot.Common
{
    public sealed class Configuration
    {
        public static readonly string Prefix = ">",
            Version = "2.0.0",
            Game = $"{Prefix}help | v{Version}",
            SupportServerLink = "https://discord.gg/MU9jaut",
            GitRepository = "https://github.com/LumiteDubbz/NukoBot",
            BotInviteLink = "https://discordapp.com/oauth2/authorize?client_id=636923604277395456&scope=bot&permissions=8",
            HelpMessage = $"Hey, I'm Nuko, a Discord bot for community management in Mayrom's new mod server.\n\nTo view all command modules, say `{Prefix}modules`.\n\nTo view all commands in any given module, say `{Prefix}module [moduleName]`.\n\n**If you encounter a bug or want to give feedback, please use the `{Prefix}support` command and join the server.**",
            DefaultWarnPunishment = "kick";

        public static readonly string[] Authors = {
            "Lumite_#0187",
            "Maycrom#7729"
        };

        public static readonly int MaximumLeaderboardPosition = 3,
            MaximumClearCount = 150,
            MinimumClearCount = 1,
            MinimumMuteLength = 1,
            TwoPlayerBonusPointPercentage = 10,
            DaysUntilWarningExpires = 30,
            MaximumWarningsBeforeAction = 2;

        public static readonly double MaximumMultiplier = 10,
            MinimumMultiplier = 0.1,
            WarnMuteLength = 24,
            WeightedPointIncrement = 0.25;

        public static readonly TimeSpan AutoUnmuteCooldown = TimeSpan.FromMinutes(1),
            AutoDeletePollsCooldown = TimeSpan.FromMinutes(1);

        public static Color Color()
        {
            return Colors[Randomizer.Next(1, Colors.Length) - 1];
        }

        public static readonly Random Randomizer = new Random();

        public static readonly Color BanColor = new Color(255, 0, 0),
            KickColor = new Color(255, 140, 25),
            MuteColor = new Color(255, 140, 25),
            UnmuteColor = new Color(85, 255, 0),
            ClearColor = new Color(85, 255, 0),
            WarnColor = new Color(255, 140, 25),
            LockdownColor = new Color(255, 0, 0),
            UnlockColor = new Color(85, 255, 0),
            ErrorColor = new Color(255, 0, 0);

        private static readonly Color[] Colors = {
            new Color(255, 38, 154),
            new Color(0, 255, 0),
            new Color(0, 232, 40),
            new Color(8, 248, 255),
            new Color(242, 38, 255),
            new Color(255, 28, 142),
            new Color(104, 255, 34),
            new Color(255, 190, 17),
            new Color(41, 84, 255),
            new Color(150, 36, 237),
            new Color(168, 237, 0)
        };
    }
}