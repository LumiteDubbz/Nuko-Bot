using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Preconditions.Command;
using NukoBot.Common.Structures;
using NukoBot.Database.Repositories;
using NukoBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    [Name("Administrator")]
    [Summary("Commands only allowed to be used by users with a role with a permission level of at least 2.")]
    [RequireAdministrator]
    public sealed class AdministratorModule : ModuleBase<Context>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;
        private readonly GuildRepository _guildRepository;
        private readonly UserRepository _userRepository;
        private readonly ModerationService _moderationService;
        private readonly PointService _pointService;

        public AdministratorModule(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
            _guildRepository = _serviceProvider.GetRequiredService<GuildRepository>();
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
            _moderationService = _serviceProvider.GetRequiredService<ModerationService>();
            _pointService = _serviceProvider.GetRequiredService<PointService>();
        }

        [Command("SetScreenshotChannel")]
        [Alias("removescreenshotchannel")]
        [Summary("Set the channel for screenshots to be submitted to.")]
        public async Task SetScreenshotChannel([Summary("The channel you want to set as the screenshot submitting channel.")][Remainder] ITextChannel screenshotChannel = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ScreenshotChannelId = screenshotChannel.Id);

            if (screenshotChannel != null)
            {
                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the screenshot channel to {screenshotChannel.Mention}.");
                return;
            }

            await _text.ReplyAsync(Context.User, Context.Channel, "you have successfully removed the screenshot channel.");
        }

        [Command("SetMutedRole")]
        [Alias("setmuterole", "removemutedrole", "removemuterole")]
        [Summary("Set the role to be given to those who are muted.")]
        public async Task SetMutedRole([Summary("The role you want to set as the muted role.")][Remainder] IRole mutedRole = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.MutedRoleId = mutedRole.Id);

            if (mutedRole != null)
            {
                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the muted role to {mutedRole.Mention}.");
                return;
            }

            await _text.ReplyAsync(Context.User, Context.Channel, "you have successfully removed the muted role.");
        }

        [Command("SetModLogChannel")]
        [Alias("setlogchannel", "setmodlog", "removemodlogchannel", "removelogchannel")]
        [Summary("Set the channel for all moderator actions to be logged to.")]
        public async Task SetModLogChannel([Summary("The channel you wish to set as the mod log channel.")][Remainder] ITextChannel modLogChannel = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ModLogChannelId = modLogChannel.Id);

            if (modLogChannel != null)
            {
                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the mod log channel to {modLogChannel.Mention}.");
                return;
            }

            await _text.ReplyAsync(Context.User, Context.Channel, "you have successfully removed the mod log channel.");
        }

        [Command("SetWelcomeMessage")]
        [Alias("setwelcomemsg", "deletewelcomemessage", "deletewelcomemsg", "removewelcomemessage", "removewelcomemsg")]
        [Summary("Set the message to sent when a user joins this server.")]
        public async Task SetWelcomeChannel([Summary("The welcome message to be DMed to new users.")][Remainder] string welcomeMessage = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.WelcomeMessage = welcomeMessage);

            if (welcomeMessage != null)
            {
                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the welcome message to **{welcomeMessage}**");
                return;
            }

            await _text.ReplyAsync(Context.User, Context.Channel, "you have successfully removed the welcome message.");
        }

        [Command("SetNewUserRole")]
        [Alias("setdefaultrole")]
        [Summary("Set the role to be given to all new users in the server.")]
        public async Task SetNewUserRole([Summary("The role to be given to new users.")][Remainder] IRole newUserRole = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.NewUserRole = newUserRole.Id);

            if (newUserRole != null)
            {
                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the new user role to {newUserRole.Mention}.");
                return;
            }

            await _text.ReplyAsync(Context.User, Context.Channel, "you have successfully removed the new user role.");
        }

        [Command("Award")]
        [Alias("awardpoints", "givepoints")]
        [Summary("Add points to a user which will also increase the glboal point counter.")]
        public async Task Award([Summary("The round the user died on.")] int round, [Summary("The difficulty map the user played on.")] string mapName = null, [Summary("The user you wish to give the points to.")] IGuildUser user = null, [Summary("Whether or not the game was played with a friend in the server.")][Remainder] bool playedWithOther = false)
        {
            if (user == null && mapName == null)
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += round);

                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully added **{round}** points to this server's total.");
            }

            mapName = mapName.ToLower();

            var dbUser = await _userRepository.GetUserAsync(user.Id, Context.Guild.Id);

            if (dbUser == null)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"that user was not found.");
                return;
            }

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
                    await _text.ReplyErrorAsync(Context.User, Context.Channel, "no map with that name was found.");
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
                    weightedPointMultiplier += 0.25;
                }
            }

            double points = Math.Ceiling(round * map.Multiplier);

            if (playedWithOther == true)
            {
                points = Math.Ceiling(points + (points * 10 / 100));
            }

            if (Context.DbGuild.PointMultiplier > 1)
            {
                points = Math.Ceiling(points * Context.DbGuild.PointMultiplier);
            }

            points = Math.Ceiling(points * weightedPointMultiplier);

            await _userRepository.ModifyAsync(dbUser, x => x.Points += (int)points);
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += (int)points);

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

            var userDm = await user.GetOrCreateDMChannelAsync();

            await _text.SendAsync(userDm, dmMessage);
            await _text.ReplyAsync(Context.User, Context.Channel, adminResponse);

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

        [Command("Deduct")]
        [Alias("deductpoints", "removepoints")]
        [Summary("Remove points from a user which will also decrease the global point counter.")]
        public async Task Deduct([Summary("The amount of points to be taken away.")] int amountOfPoints, [Summary("The user you want to take ponts away from.")] IGuildUser user = null, [Remainder] string reason = null)
        {
            if (user == null)
            {
                if (Context.DbGuild.Points - amountOfPoints < 0)
                {
                    await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points = 0);
                }
                else
                {
                    await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points -= amountOfPoints);
                }

                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully removed **{amountOfPoints}** from this guild's total.");
            }
            else
            {
                var dbUser = await _userRepository.GetUserAsync(user.Id, Context.Guild.Id);

                if (dbUser == null)
                {
                    await _text.ReplyErrorAsync(Context.User, Context.Channel, $"that user was found.");
                    return;
                }

                if (Context.DbUser.Points - amountOfPoints < 0)
                {
                    await _userRepository.ModifyAsync(dbUser, x => x.Points = 0);
                }
                else
                {
                    await _userRepository.ModifyAsync(dbUser, x => x.Points -= amountOfPoints);
                }

                if (Context.DbGuild.Points - amountOfPoints < 0)
                {
                    await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points = 0);
                }
                else
                {
                    await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points -= amountOfPoints);
                }

                await _pointService.HandleRanksAsync(user, Context.DbGuild, dbUser);

                var userDm = await user.GetOrCreateDMChannelAsync();
                var message = $"**{Context.User.Mention}** has deducted **{amountOfPoints}** points from you in **{Context.Guild.Name}**";

                message += reason != null ? $" for **{reason}**." : ".";

                await _text.SendAsync(userDm, message);

                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully removed **{amountOfPoints}** from {user.Mention}.");
            }
        }

        [Command("Ban")]
        [Alias("banish")]
        [Summary("Ban any user from being in the server.")]
        public async Task Ban([Summary("The user to ban.")] IGuildUser userToBan, [Summary("The reason for banning the user.")][Remainder] string reason = null)
        {
            if (_moderationService.GetPermissionLevel(Context.DbGuild, userToBan) > 0)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"{userToBan.Mention} is a moderator and thus cannot be banned.");
                return;
            }

            string message = $"{Context.User.Mention} has banned you from **{Context.Guild.Name}**";
            string reply = $"You have successfully banned {userToBan.Mention}";

            if (reason.Length > 0)
            {
                message += $" for **{reason}**";
                reply += $" for **{reason}**";
            }

            await _moderationService.InformUserAsync(userToBan, message + ".");

            await userToBan.BanAsync(0, reason);

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Ban", Configuration.KickColor, reason, Context.User as IGuildUser, userToBan);

            await _text.ReplyAsync(Context.User, Context.Channel, reply + ".");
        }

        [Command("SetTop3Role")]
        [Alias("settoprole", "setlbrole", "setleaderboardrole", "settop3")]
        [Summary("Set the role to be given to the 3 users with the most points.")]
        public async Task SetTopThreeRole([Summary("The role to be given.")][Remainder] IRole topThreeRole)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.TopThreeRole = topThreeRole.Id);

            if (topThreeRole != null)
            {
                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the top 3 role to {topThreeRole.Mention}.");
                return;
            }

            await _text.ReplyAsync(Context.User, Context.Channel, "you have successfully removed the top 3 role.");
        }

        [Command("SetPointMultiplier")]
        [Alias("setmultiplier", "setguildmultiplier", "setguildpointmultiplier", "setservermultiplier", "setserverpointmultiplier", "removemultiplier", "removepointmultiplier")]
        [Summary("Set the multiplier that all awarded points will be multiplied by until set to default")]
        public async Task SetPointMultiplier([Summary("The point multiplier.")][Remainder] double multiplier = 1)
        {
            if (multiplier > Configuration.MaximumMultiplier || multiplier < Configuration.MinimumMultiplier)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"you must set a multiplier between {Configuration.MinimumMultiplier}x and {Configuration.MaximumMultiplier}x.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.PointMultiplier = multiplier);

            if (multiplier > 1)
            {
                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set this server's point multiplier to {multiplier}x.");
                return;
            }

            await _text.ReplyAsync(Context.User, Context.Channel, "you have successfully set the point multiplier to default.");
        }

        [Command("CustomAward")]
        [Alias("customawardpoints", "customgive")]
        [Summary("Give any points to any user, irrespective of milestones or map difficulty multipliers.")]
        public async Task CustomAward([Summary("The amount of points to be awarded.")] int points, [Summary("The user to award points to")][Remainder] IGuildUser user = null)
        {
            if (user == null)
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += points);

                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully added **{points}** points to this server's total.");
                return;
            }

            var dbUser = await _userRepository.GetUserAsync(user.Id, user.GuildId);

            await _userRepository.ModifyAsync(dbUser, x => x.Points += points);
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += points);
            await _pointService.HandleRanksAsync(user, Context.DbGuild, dbUser);

            var dmMessage = $"**{Context.User.Mention}** has awarded you **{points}** points in **{Context.Guild.Name}**.";
            var adminResponse = $"you have successfully added **{points}** points to {user.Mention}.";
            var userDm = await user.GetOrCreateDMChannelAsync();

            await _text.SendAsync(userDm, dmMessage);
            await _text.ReplyAsync(Context.User, Context.Channel, adminResponse);

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

        [Command("AddRank")]
        [Alias("createrank", "addrankrole")]
        [Summary("Add a rank role that users are awarded upon receiving the neccesary amount of points.")]
        public async Task AddRank(IRole role, double pointsRequired)
        {
            if (role.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"My role must be higher in the role order than **{role.Mention}**.");
                return;
            }

            if (Context.DbGuild.RankRoles == null || Context.DbGuild.RankRoles.ElementCount == 0)
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.RankRoles.Add(role.Id.ToString(), pointsRequired));
            }
            else
            {
                if (Context.DbGuild.RankRoles.Any(x => x.Name == role.Id.ToString()))
                {
                    await _text.ReplyErrorAsync(Context.User, Context.Channel, $"**{role.Mention}** is already a rank role.");
                    return;
                }

                if (Context.DbGuild.RankRoles.Any(x => (int)x.Value.AsDouble == (int)pointsRequired))
                {
                    await _text.ReplyErrorAsync(Context.User, Context.Channel, "a rank with that point requirement already exists.");
                    return;
                }

                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.RankRoles.Add(role.Id.ToString(), pointsRequired));
            }

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully added **{role.Mention}** as a rank role with a point requirement of **{pointsRequired}**.");
        }

        [Command("RemoveRank")]
        [Alias("deleterank", "deleterankrole")]
        [Summary("Remove a rank role.")]
        public async Task RemoveRank(IRole role)
        {
            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "this server has no rank roles.");
                return;
            }

            if (!Context.DbGuild.RankRoles.Any(x => x.Name == role.Id.ToString()))
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"**{role.Mention}** is not a rank role.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.RankRoles.Remove(role.Id.ToString()));

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully removed **{role.Mention}** from this server's rank roles.");
        }

        [Command("ModifyRank")]
        [Alias("editrank", "editrankrole", "modifyrankrole")]
        [Summary("Modify the point requirement for any rank role.")]
        public async Task ModifyRank(IRole role, double pointsRequired)
        {
            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "this server has no rank roles.");
                return;
            }
            
            if (!Context.DbGuild.RankRoles.Any(x => x.Name == role.Id.ToString()))
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"**{role.Mention}** is not a rank role.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.RankRoles[role.Id.ToString()] = pointsRequired);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully updated the **{role.Mention}** rank to have a point requirement of **{pointsRequired}**.");
        }
    }
}