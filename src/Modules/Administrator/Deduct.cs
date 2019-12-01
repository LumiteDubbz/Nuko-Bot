using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
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

                await ReplyAsync($"you have successfully removed **{amountOfPoints}** from this guild's total.");
            }
            else
            {
                var dbUser = await _userRepository.GetUserAsync(user.Id, Context.Guild.Id);

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

                var message = $"**{Context.User.Mention}** has deducted **{amountOfPoints}** points from you in **{Context.Guild.Name}**";

                message += reason != null ? $" for **{reason}**." : ".";

                await DmAsync(user, message);

                await ReplyAsync($"you have successfully removed **{amountOfPoints}** from {user.Mention}.");
            }
        }
    }
}
