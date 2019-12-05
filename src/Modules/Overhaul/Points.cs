using Discord;
using Discord.Commands;
using NukoBot.Common.Structures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Overhaul
{
    public partial class Overhaul
    {
        [Command("Points")]
        [Alias("pointcount", "me", "self")]
        [Summary("View the amount of points you or a mentioned user has.")]
        [Remarks("@Cheese fries#8263")]
        public async Task Points([Summary("The user you want to look at the results of.")][Remainder] IUser user = null)
        {
            if (!Context.DbGuild.OverhaulEnabled)
            {
                await ReplyErrorAsync("all Overhaul related commands are disabled on this server.");
                return;
            }

            var allMilestones = new List<Milestone>();

            if (user != null)
            {
                var dbUser = await _userRepository.GetUserAsync(user.Id, Context.Guild.Id);
                var otherUserMessage = $"{user.Mention} has **{dbUser.Points}** points, contributing to this guild's total of **{Context.DbGuild.Points}** points.";
                var userRank = await _pointService.GetRankAsync(Context, dbUser);

                if (userRank != null)
                {
                    otherUserMessage += $"\n\nTheir rank is **{userRank.Mention}**.";
                }

                if (dbUser.Milestones.Any())
                {
                    foreach (var milestone in dbUser.Milestones)
                    {
                        allMilestones.Add(milestone);
                    }
                }

                var groupedMilestones = allMilestones.GroupBy(x => x.MapName).Select(g => (MapName: g.Key, HighestRound: g.Max(x => x.Round))).ToList();

                var otherUserMilestones = "Their highest rounds on each map are:\n";

                foreach (var milestone in groupedMilestones)
                {
                    otherUserMilestones += $"{milestone.MapName}: **{milestone.HighestRound}**\n";
                }

                if (otherUserMilestones != "Their highest rounds on each map are:\n")
                {
                    otherUserMessage += $"\n\n{otherUserMilestones.Remove(otherUserMilestones.Length - 1)}";
                }

                await ReplyAsync(otherUserMessage);
                return;
            }

            var message = $"you have **{Context.DbUser.Points}** points, contributing to this guild's total of **{Context.DbGuild.Points}** points.";
            var selfRank = await _pointService.GetRankAsync(Context, Context.DbUser);

            if (selfRank != null)
            {
                message += $"\n\nYour rank is **{selfRank.Mention}**.";
            }

            if (Context.DbUser.Milestones.Any())
            {
                foreach (var milestone in Context.DbUser.Milestones)
                {
                    allMilestones.Add(milestone);
                }
            }

            var groupedMilestonesUser = allMilestones.GroupBy(x => x.MapName).Select(g => (MapName: g.Key,HighestRound: g.Max(x => x.Round))).ToList();

            var milestones = "Your highest rounds on each map are:\n";

            foreach (var milestone in groupedMilestonesUser)
            {
                milestones += $"{milestone.MapName}: **{milestone.HighestRound}**\n";
            }

            if (milestones != "Your highest rounds on each map are:\n")
            {
                message += $"\n\n{milestones.Remove(milestones.Length - 1)}";
            }

            await ReplyAsync(message);
        }
    }
}
