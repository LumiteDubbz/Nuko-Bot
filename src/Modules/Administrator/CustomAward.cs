using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("CustomAward")]
        [Alias("customawardpoints", "customgive")]
        [Summary("Give any points to any user, irrespective of milestones or map difficulty multipliers.")]
        [Remarks("30 \"@Blaze King#9142\"")]
        public async Task CustomAward([Summary("The amount of points to be awarded.")] int points, [Summary("The user to award points to")][Remainder] IGuildUser user = null)
        {
            if (!Context.DbGuild.OverhaulEnabled)
            {
                await ReplyErrorAsync("all Overhaul related commands are disabled on this server.");
                return;
            }

            if (user == null)
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += points);

                await ReplyAsync($"you have successfully added **{points}** points to this server's total.");
                return;
            }

            var dbUser = await _userRepository.GetUserAsync(user.Id, user.GuildId);

            await _userRepository.ModifyAsync(dbUser, x => x.Points += points);
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += points);
            await _pointService.HandleRanksAsync(user, Context.DbGuild, dbUser);

            var dmMessage = $"**{Context.User.Mention}** has awarded you **{points}** points in **{Context.Guild.Name}**.";
            var adminResponse = $"you have successfully added **{points}** points to {user.Mention}.";

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
