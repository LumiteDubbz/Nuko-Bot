using Discord;
using Discord.Commands;
using NukoBot.Common;
using NukoBot.Common.Structures;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("Award")]
        [Alias("awardpoints", "givepoints")]
        [Summary("Add points to a user which will also increase the glboal point counter.")]
        [Remarks("44 easy01 \"@Jesus Christ#0000\" true")]
        public async Task Award([Summary("The round the user died on.")] int round, [Summary("The difficulty map the user played on.")] string mapName = null, [Summary("The user you wish to give the points to.")] IGuildUser user = null, [Summary("Whether or not the game was played with a friend in the server.")][Remainder] bool playedWithOther = false)
        {
            if (user == null)
            {
                if (mapName == null)
                {
                    await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += round);

                    await ReplyAsync($"you have successfully added **{round}** points to this server's total.");
                }
                else
                {
                    await ReplyErrorAsync("you need to provide a user.");
                    return;
                }
            }

            mapName = mapName.ToLower();

            Map map;

            switch (mapName)
            {
                case "easy01":
                    map = Maps.Easy01Map;
                    break;
                case "easy02":
                    map = Maps.Easy02Map;
                    break;
                case "normal01":
                    map = Maps.Normal01Map;
                    break;
                case "normal02":
                    map = Maps.Normal02Map;
                    break;
                case "hard01":
                    map = Maps.Hard01Map;
                    break;
                case "hard02":
                    map = Maps.Hard02Map;
                    break;
                default:
                    await ReplyErrorAsync("no map with that name was found.");
                    return;
            }

            var passedMilestones = new List<Milestone>();
            var weightedPointMultiplier = 1.0;

            foreach (var milestone in map.Milestones)
            {
                if (round < milestone.Round) continue;

                passedMilestones.Add(milestone);

                if (milestone.Round % 10 == 0 && milestone.Round != 10)
                {
                    weightedPointMultiplier += Configuration.WeightedPointIncrement;
                }
            }

            var points = await _pointService.GetPointAmountAsync(round, weightedPointMultiplier, map.Multiplier, Context.DbGuild.PointMultiplier, playedWithOther);
            var dbUser = await _userRepository.GetUserAsync(user.Id, Context.Guild.Id);

            await _userRepository.ModifyAsync(dbUser, x => x.Points += points);
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += points);

            var dmMessage = $"**{Context.User.Mention}** has awarded you **{points}** points in **{Context.Guild.Name}**.";
            var adminResponse = $"you have successfully added **{points}** points to {user.Mention}.";

            int bonusPoints = 0;

            if (passedMilestones.Any())
            {
                var passedMilestonesOrdered = passedMilestones.OrderByDescending(x => x.Round);
                var highestMilestone = passedMilestonesOrdered.First();
                var nextHighestMilestone = passedMilestonesOrdered.ElementAtOrDefault(1);

                var newMilstone = new Milestone()
                {
                    MapName = highestMilestone.MapName,
                    PointBonus = highestMilestone.PointBonus,
                    Round = round
                };

                var existingMilestoneOnMap = dbUser.Milestones.OrderByDescending(x => x.Round).Where(x => x.MapName == map.Name).FirstOrDefault();

                if (existingMilestoneOnMap != null)
                {
                    if (existingMilestoneOnMap.Round >= round)
                    {
                        bonusPoints = 0;
                    }
                    else
                    {
                        bonusPoints = highestMilestone.PointBonus;

                        await _userRepository.ModifyAsync(dbUser, x => x.Milestones.Add(newMilstone));
                    }
                }
                else
                {
                    bonusPoints = highestMilestone.PointBonus;
                }

                await _userRepository.ModifyAsync(dbUser, x => x.Milestones.Add(newMilstone));
            }

            if (bonusPoints > 0)
            {
                await _userRepository.ModifyAsync(dbUser, x => x.Points += bonusPoints);
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += bonusPoints);

                dmMessage += $"\n\nYou were also awarded **{bonusPoints}** bonus points for the round you got up to. Remember that these are one-time only, but if you get higher, you can get more!";
                adminResponse += $"\n\nThe user was also awarded **{bonusPoints}** bonus points for the round milestone they achieved.";
            }

            await DmAsync(user, dmMessage);
            await ReplyAsync(adminResponse);

            if (Context.DbGuild.TopThreeRole != 0)
            {
                var thirdPlaceDbUser = (await _userRepository.AllAsync(x => x.GuildId == Context.Guild.Id)).OrderByDescending(x => x.Points).ElementAtOrDefault(2);
                var fourthPlaceDbUser = (await _userRepository.AllAsync(x => x.GuildId == Context.Guild.Id)).OrderByDescending(x => x.Points).ElementAtOrDefault(3);

                if (thirdPlaceDbUser == default)
                {
                    await user.AddRoleAsync(Context.Guild.GetRole(Context.DbGuild.TopThreeRole));
                    return;
                }

                if (dbUser.Points >= thirdPlaceDbUser.Points)
                {
                    await user.AddRoleAsync(Context.Guild.GetRole(Context.DbGuild.TopThreeRole));

                    if (fourthPlaceDbUser == default || thirdPlaceDbUser.Points <= fourthPlaceDbUser.Points)
                    {
                        var thirdPlaceGuildUser = (IGuildUser)Context.Guild.GetUser(thirdPlaceDbUser.UserId);

                        await thirdPlaceGuildUser.AddRoleAsync(Context.Guild.GetRole(Context.DbGuild.TopThreeRole));
                    }
                }
            }
        }
    }
}
