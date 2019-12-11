using Discord;
using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.Moderator
{
    public partial class Moderator
    {
        [Command("Unmute")]
        [Alias("unsilence")]
        [Summary("Unmute a muted user.")]
        [Remarks("@Ass_Ass#2212 Nevermind")]
        public async Task Unmute([Summary("The user you want to unmute.")] IGuildUser userToUnmute, [Remainder] string reason = null)
        {
            if (!(await _muteRepository.AnyAsync(x => x.UserId == userToUnmute.Id && x.GuildId == Context.Guild.Id)))
            {
                await ReplyErrorAsync("that user is not muted and thus cannot be unmuted.");
                return;
            }

            await _muteRepository.DeleteAsync(x => x.UserId == userToUnmute.Id && x.GuildId == Context.Guild.Id);

            await userToUnmute.RemoveRoleAsync(Context.Guild.GetRole(Context.DbGuild.MutedRoleId));

            await ReplyAsync($"you have successfully unmuted **{userToUnmute.Mention}**.");

            string message = $"**{Context.User.Mention}** has unmuted you in **{Context.Guild.Name}**";

            if (!string.IsNullOrWhiteSpace(reason))
            {
                message += $" for **{reason}**";
            }

            await _moderationService.InformUserAsync(userToUnmute, message + ".");

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Unmute", Configuration.UnmuteColor, reason, Context.User as IGuildUser, userToUnmute);
        }
    }
}
