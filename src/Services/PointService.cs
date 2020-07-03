using Discord.WebSocket;
using Discord;
using NukoBot.Common;
using NukoBot.Database.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace NukoBot.Services
{
    public sealed class PointService
    {
        public async Task HandleRanksAsync(IGuildUser user, Guild dbGuild, User dbUser)
        {
            if (dbGuild.RankRoles.ElementCount != 0)
            {
                var currentUser = await user.Guild.GetCurrentUserAsync() as SocketGuildUser;

                decimal points = dbUser.Points;

                var rolesToAdd = new List<IRole>();
                var rolesToRemove = new List<IRole>();
                var highestRolePosition = currentUser.Roles.OrderByDescending(x => x.Position).First().Position;

                foreach (var rankRole in dbGuild.RankRoles)
                {
                    var pointsRequired = (decimal)rankRole.Value.AsDouble;
                    var role = user.Guild.GetRole(ulong.Parse(rankRole.Name));

                    if (role != null && role.Position < highestRolePosition)
                    {
                        var hasRole = user.RoleIds.Any(x => x == role.Id);

                        if (points >= pointsRequired && !hasRole)
                        {
                            rolesToAdd.Add(role);
                        }

                        if (points < pointsRequired && hasRole)
                        {
                            rolesToRemove.Add(role);
                        }
                    }
                }

                foreach (var role in rolesToAdd)
                {
                    await user.AddRoleAsync(role);
                }

                foreach (var role in rolesToRemove)
                {
                    await user.RemoveRoleAsync(role);
                }
            }
        }

        public Task<IRole> GetRankAsync(Context context, User dbUser)
        {
            IRole role = null;

            if (context.DbGuild.RankRoles.ElementCount != 0 && context.Guild != null)
            {
                foreach (var rankRole in context.DbGuild.RankRoles.OrderBy(x => x.Value))
                {
                    if (dbUser.Points >= (decimal) rankRole.Value.AsDouble)
                    {
                        role = context.Guild.GetRole(ulong.Parse(rankRole.Name));
                    }
                }
            }

            return Task.FromResult(role);
        }

        public Task<int> GetPointAmountAsync(int round, double weightedPointMultiplier, double mapMultiplier, double guildMultiplier, bool twoPlayerBonus)
        {
            double total = round;

            total *= weightedPointMultiplier;
            total *= mapMultiplier;
            total *= guildMultiplier;
            total = twoPlayerBonus ? total + (total / Configuration.TwoPlayerBonusPointPercentage) : total;

            return Task.FromResult((int) Math.Ceiling(total));
        }
    }
}
