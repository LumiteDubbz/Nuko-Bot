using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Preconditions.Command;
using NukoBot.Database.Repositories;
using NukoBot.Database.Models;
using NukoBot.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using NukoBot.Common.Structures;
using System.Net;
using Newtonsoft.Json;

namespace NukoBot.Modules.General
{
    [Name("General")]
    [Summary("General commands not directly related to the core functionality of the bot.")]
    [RequireContext(ContextType.Guild)]
    public sealed class GeneralModule : ModuleBase<Context>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;
        private readonly UserRepository _userRepository;
        private readonly PollRepository _pollRepository;

        public GeneralModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
            _pollRepository = _serviceProvider.GetRequiredService<PollRepository>();
        }

        [Command("Submit")]
        [Alias("submit-screenshot")]
        [Summary("Submit a screenshot of your game to the staff for points.")]
        [RequireAttachedImage]
        public async Task Submit()
        {
            if (Context.Message.Attachments.ElementAt(0) == null)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "an unknown error occurred while submitting that image.");
                return;
            }

            await _text.SendScreenshotAsync(Context.Client.GetChannel(Context.DbGuild.ScreenshotChannelId) as IMessageChannel, $"**{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}** has submitted this screenshot.", Context.Message.Attachments.ElementAt(0));

            var message = await ReplyAsync($"You have successfully submitted your screenshot for review.");

            await Task.Delay(2000);

            await Context.Message.DeleteAsync();

            await message.DeleteAsync();
        }

        [Command("Points")]
        [Alias("pointcount", "me")]
        [Summary("View the amount of points you or a mentioned user has.")]
        public async Task Points([Summary("The user you want to look at the results of.")][Remainder] IUser user = null)
        {
            var allMilestones = new List<Milestone>();

            if (user != null)
            {
                var dbUser = await _userRepository.GetUserAsync(user.Id, Context.Guild.Id);

                var otherUserMessage = $"{user.Mention} has **{dbUser.Points}** points, contributing to this guild's total of **{Context.DbGuild.Points}** points.";

                if (dbUser.Milestones.Any())
                {
                    foreach (var milestone in dbUser.Milestones)
                    {
                        allMilestones.Add(milestone);
                    }
                }

                var groupedMilestones = allMilestones.GroupBy(x => x.MapName).Select(g => new {
                    MapName = g.Key,
                    HighestRound = g.Max(x => x.Round)
                }).ToList();
                var otherUserMilestones = "Their highest rounds on each map are:\n";

                foreach (var milestone in groupedMilestones)
                {
                    otherUserMilestones += $"{milestone.MapName}: **{milestone.HighestRound}**\n";
                }

                if (otherUserMilestones != "Their highest rounds on each map are:\n")
                {
                    otherUserMessage += $"\n\n{otherUserMilestones.Remove(otherUserMilestones.Length - 1)}";
                }

                await _text.ReplyAsync(Context.User, Context.Channel, otherUserMessage);
                return;
            }

            var message = $"you have **{Context.DbUser.Points}** points, contributing to this guild's total of **{Context.DbGuild.Points}** points.";

            if (Context.DbUser.Milestones.Any())
            {
                foreach (var milestone in Context.DbUser.Milestones)
                {
                    allMilestones.Add(milestone);
                }
            }

            var groupedMilestonesUser = allMilestones.GroupBy(x => x.MapName).Select(g => new {
                MapName = g.Key,
                HighestRound = g.Max(x => x.Round)
            }).ToList();
            var milestones = "Your highest rounds on each map are:\n";

            foreach (var milestone in groupedMilestonesUser)
            {
                milestones += $"{milestone.MapName}: **{milestone.HighestRound}**\n";
            }

            if (milestones != "Your highest rounds on each map are:\n")
            {
                message += $"\n\n{milestones.Remove(milestones.Length - 1)}";
            }

            await _text.ReplyAsync(Context.User, Context.Channel, message);
        }

        [Command("Vote")]
        [Alias("votepoll")]
        [Summary("Vote on any active poll")]
        public async Task Vote([Summary("The number corresponding to the poll you want to delete.")] int pollIndex, [Summary("The number corresponding to you choice you want to vote for.")] int choiceIndex)
        {
            var poll = await _pollRepository.GetPollAsync(pollIndex, Context.Guild.Id);

            if (poll != null)
            {
                if (poll.VotesDocument.Any(x => x.Name == Context.User.Id.ToString()))
                {
                    await _text.ReplyErrorAsync(Context.User, Context.Channel, "you have already voted on this poll.");
                    return;
                }

                string choice = null;

                try
                {
                    choice = poll.Choices[choiceIndex - 1];
                }
                catch (IndexOutOfRangeException)
                {
                    await _text.ReplyErrorAsync(Context.User, Context.Channel, "no vote with that index (ID) was found.");
                }

                await _pollRepository.ModifyAsync(poll, x => x.VotesDocument.Add(Context.User.Id.ToString(), choice));

                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully voted on **{poll.Name}**.");
                return;
            }

            await _text.ReplyErrorAsync(Context.User, Context.Channel, "no poll with that index (ID) was found.");
        }

        [Command("Leaderboard")]
        [Alias("top", "topusers", "lb")]
        [Summary("View 3 users with the most points in the server.")]
        public async Task Leaderboard()
        {
            var users = (await _userRepository.AllAsync(x => x.GuildId == Context.Guild.Id)).OrderByDescending(x => x.Points);

            var message = string.Empty;

            int position = 1;

            if (users.Count() == 0)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "there are no users on the learderboard yet.");
                return;
            }

            var guildInterface = Context.Guild as IGuild;

            foreach (User dbUser in users)
            {
                var user = await guildInterface.GetUserAsync(dbUser.UserId);

                if (user == null)
                {
                    continue;
                }

                message += $"**{position}**. {user.Mention}: {dbUser.Points} points\n";

                if (position >= Configuration.MaximumLeaderboardPosition)
                {
                    break;
                }

                position++;
            }

            await _text.SendAsync(Context.Channel, message);
        }

        [Command("Echo")]
        [Alias("say", "embed")]
        [Summary("Repeat the provided text in an embedded message.")]
        public async Task Echo([Summary("The text you want the bot to embed.")][Remainder] string message)
        {
            await _text.SendAsync(Context.Channel, message);
        }

        //[Command("Define")]
        //[Alias("dictionary")]
        //[Summary("Search for the definition of a word.")]
        //public async Task Define([Summary("The word/s you want to search for.")][Remainder] string word)
        //{
        //    var webClient = new WebClient();
        //    var url = $"https://dictionaryapi.com/api/v3/references/collegiate/json/{word}?key=aaaa";
        //    var file = webClient.DownloadString(url);
        //}
    }
}