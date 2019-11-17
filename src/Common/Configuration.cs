using Discord;
using NukoBot.Common.Structures;
using System;
using System.Collections.Generic;

namespace NukoBot.Common
{
    public sealed class Configuration
    {
        public static readonly string Prefix = ">";

        public static readonly string Version = "1.3.15";

        public static readonly string Game = Prefix + "help" + $" | v{Version}";

        public static readonly string SupportServerLink = "https://discord.gg/MU9jaut", BotInviteLink = "https://discordapp.com/oauth2/authorize?client_id=636923604277395456&scope=bot&permissions=8", HelpMessage = "Hey, I'm Nuko, a Discord bot for community management in Mayrom's new mod server.\n\nTo view all command modules, say `>modules`.\n\nTo view all commands in any given module, say `>module [moduleName]`.\n\n**If you encounter a bug or want to give feedback, please use the `>support` command and join the server.**";

        public static readonly string[] Authors = {
            "Lumite_#0187",
            "Maycrom#7729"
        };

        public static readonly int MaximumLeaderboardPosition = 3;

        public static readonly double MaximumMultiplier = 10;

        public static readonly double MinimumMultiplier = 0.1;

        public static List<HelpResponse> AutomaticHelpResponses = new List<HelpResponse> {
            new HelpResponse() {
                TriggerWords = {
                    "How do I install the mod",
                    "How do I install mods",
                    "How do I install",
                    "How to install mods",
                    "How do I get the mod",
                    "How to get the mod",
                    "Where do I get the mod",
                    "Where do I download the mod",
                    "How do you download the mod"
                },
                Response = "**You might want to use another Steam account to prevent your main from being banned!**\n\nPlease note that the mod is only currently available for Windodws Steam.\n\nTo download the mod: Download the latest ZIP file from `#download`.\n\nTo install the mod:\n1) Extract the ZIP file.\n2) Open a new file explorer window.\n3)Go to `C:\\Program Files (x86)\\Steam\\steamapps\\common\\Bloons TD Battles`\n4) Copy the `Assets` and `ExtraAssets` folders from the mod folder and paste it into the BTD Battles folder.\n5) Overwrite any files it asks you to."
            },
            
            new HelpResponse() {
                TriggerWords = {
                    "How do points work",
                    "How do I get points",
                    "How do I submit screenshots",
                    "Where do I get points",
                    "Where do I submit screenshots",
                    "What are points",
                    "How exactly does the point system award points",
                    "What do points do",
                    "What can I spend points on",
                    "How do I use points"
                },
                Response = "You may submit screenshots in #submit-screenshots using the command `>submit` with your screenshot attached.\n\nThe administrators will look at your screenshot and award you points based on 3 factors:\nThe round you died on,\nThe difficulty of the map you played on and\nIf you played with someone else in the server.\n\nIf you played on the easy map, you get 75% of the round you got in points. On the normal map you get 100% of the round you got in points. On the hard map, you get 125% of the round you got in points.\n\nIf you played with someone else in the server and they also submit a screenshot, you both get 10% bonus points.\n\nThe server's total point count (which you can view by using `>points`) can be used to upgrade certain things. To view these, say \"what upgrades are there\"."
            },
            
            new HelpResponse() {
                TriggerWords = {
                    "What upgrades are there",
                    "What upgrades will there be",
                    "What upgrades can we get",
                    "Where can I see the upgrades",
                    "What are upgrades",
                    "What are the upgrades"
                },
                Response = "You can view the available upgrades from this spreadsheet: https://docs.google.com/spreadsheets/d/1v-MLviIsb9qTvKPoLmsrk7lfxnM8vkC2r6lKVjE_ObQ/edit - remember to check out the other tabs to view the different types of upgrade."
            },
            
            new HelpResponse() {
                TriggerWords = {
                    "What towers are there",
                    "What enemies are there"
                },
                Response = "You can view the towers and enemies in these documents:\n\nTowers: https://docs.google.com/document/d/1b5doat4o_88qCAkb-wXcW1VWqMzjXodTMT1PI9E6NKY/edit\nEnemies:https://docs.google.com/document/d/1ZeV_ZCo-cLXE0zp1zkyI1epP4aciqKADz9T57bXwOwE/edit"
            },

            new HelpResponse() {
                TriggerWords = {
                    "What does no data found mean ",
                    "What is the hacker pool",
                    "Why am I in the hacker pool",
                    "How do I get out of the hacker pool",
                    "Am I in the hacker pool"
                },
                Response = "\"No data found\" means you're in the hacker pool.\n\nThe hacker pool is a queue for hackers/modders only, meaning you can't play against normal players.\n\nIf you want to get out of the hacker pool, you must go to Ninja Kiwi's support page, but there is a chance they won't get you out.\n\nTo avoid being put in the hacker pool, use a second Steam account to use mods."
            },
            
            new HelpResponse() {
                TriggerWords = {
                    "I have missing textures",
                    "I have a bunch of missing textures",
                    "I have a bunch of purple textures",
                    "I have loads of purple icons",
                    "My icons are broken",
                    "My textures are broken",
                    "How do I reset my game files",
                    "How to reset game files"
                },
                Response = "If you have missing textures, use Steam to verify your game files (resetting them to default) and then reinstall the mod. You can say \"How do I install the mod?\" to get the instructions for installing the mod.\n\nTo verify game files:\n1) Open Steam.\n2) Right click on Bloons TD Battles.\n3) Click \"Properties...\"\n4) Click the \"local files\" tab in the top of the properties movie.\n5) Click \"Verify integrity of game files\"."
            },
            
            new HelpResponse() {
                TriggerWords = {
                    "What will the next update have",
                    "When is the next update",
                    "When will the next update come out",
                    "What features will the next update have",
                    "What will the next update feature"
                },
                Response = "The next update will come out when the server gets enough points for an upgrade and after the community votes on what upgrade for the points to be spent on.\n\nAfter the vote, the upgrade (and other features like new maps, towers, enemies, balancing, etc.) will be developed and then released.\n\nTo learn more about points and submitting screenshots, just ask \"what are points\".\n\nTo learn more about the upgrades, just ask \"what upgrades are there\"."
            },
            
            new HelpResponse() {
                TriggerWords = {
                    "What are milestones",
                    "What do milestones do",
                    "How do I get milestones",
                    "How do I see the milestones",
                    "What are bonus points",
                    "What milestones are there"
                },
                Response = "Milestones are bonus points awarded one-time only for getting a certain round on any map. If you get round 10 on easy 01, you'll get some bonus points. The next time you get round 10 on that map you will not. The closest next milestones are 20, then 26, then 30 and onwards. They are every 10 rounds and every round after a boss wave. Milestones are unique to each map."
            },

            new HelpResponse()
            {
                TriggerWords =
                {
                    "How do I reinstall the game",
                    "How do I reinstall Battles",
                    "How do I reset my game",
                    "How do I reset my game files"
                },
                Response = "1) Open Steam.\n2) Right click on Bloons TD Battles.\n3) Click \"Properties...\"\n4) Click the \"local files\" tab in the top of the properties movie.\n5) Click \"Verify integrity of game files\"."
            },

            new HelpResponse()
            {
                TriggerWords =
                {
                    "Is this mod for android",
                    "Is there an adroid version",
                    "Can I get this mod on android",
                    "How do I install on android",
                    "Is this mod for MacOS",
                    "Is there a MacOS version",
                    "Can I get this mod on MacOS",
                    "How do I install on MacOS"
                },
                Response = "There is currently no version of this mod for android, nor is an official one planned.\n\nThere are ideas for a MacOS port in the works."
            },

            new HelpResponse()
            {
                TriggerWords =
                {
                    "My game crashes on startup",
                    "My game crashes",
                    "My game is crashing",
                    "Why does my game crash",
                    "Why does my game crash on startup"
                },
                Response = "**If you experience crashes after installing:**\n\nYou need to be on Windows Steam.\n\nYou need to reset your game files to default before installing this mod.\n\nYou need to install the mod following the instructions carefully.\n\nTo reset your game files, ask \"How do I reset my game files\". To learn how to install the mod, ask \"How do I install the mod.\""
            }
        };

        public static Color Color()
        {
            return Colors[Randomizer.Next(1, Colors.Length) - 1];
        }

        public static readonly Random Randomizer = new Random();

        public static readonly Color BanColor = new Color(255, 0, 0);

        public static readonly Color KickColor = new Color(255, 140, 25);

        public static readonly Color MuteColor = new Color(255, 140, 25);

        public static readonly Color UnmuteColor = new Color(85, 255, 0);

        public static readonly Color ClearColor = new Color(85, 255, 0);

        public static readonly Color ErrorColor = new Color(255, 0, 0);

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