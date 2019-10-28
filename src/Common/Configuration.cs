using Discord;
using System;

namespace NukoBot.Common
{
    public sealed class Configuration
    {
        public static readonly string Prefix = ">";

        public static readonly string Game = Prefix + "help";

        public static readonly string SupportServerLink = "https://discord.gg/MU9jaut", BotInviteLink = "https://discordapp.com/oauth2/authorize?client_id=636923604277395456&scope=bot&permissions=8", HelpMessage = "Hey, I'm Nuko, a Discord bot for community management in Mayrom's new mod server.\n\nTo view all command modules, say `>modules`.\n\nTo view all commands in any given module, say `>module [moduleName]`.\n\n**If you encounter a bug or want to give feedback, please use the `>support` command and join the server.**";

        public static Color Color()
        {
            return Colors[Randomizer.Next(1, Colors.Length) - 1];
        }

        public static readonly Random Randomizer = new Random();

        public static readonly Color BanColor = ErrorColor;

        public static readonly Color KickColor = MuteColor;

        public static readonly Color MuteColor = new Color(255, 140, 25);

        public static readonly Color ErrorColor = new Color(255, 0, 0);

        private static readonly Color[] Colors =
        {
            new Color(255, 38, 154), new Color(0, 255, 0), new Color(0, 232, 40), new Color(8, 248, 255), new Color(242, 38, 255), new Color(255, 28, 142), new Color(104, 255, 34), new Color(255, 190, 17), new Color(41, 84, 255), new Color(150, 36, 237), new Color(168, 237, 0)
        };
    }
}
