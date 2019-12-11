using Discord;
using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.Moderator
{
    public partial class Moderator
    {
        [Command("Kick")]
        [Alias("boot")]
        [Summary("Kick any player from the server.")]
        [Remarks("\"@Magnesium Oxide#2416\" Submitted a faked screenshot")]
        public async Task Kick([Summary("The user you wish to kick.")] IGuildUser userToKick, [Summary("The reason for kicking the user.")][Remainder] string reason = null)
        {
            if (_moderationService.GetPermissionLevel(Context.DbGuild, userToKick) > 0)
            {
                await ReplyErrorAsync($"**{userToKick.Mention}** is a moderator and thus cannot be kicked.");
                return;
            }

            string message = $"**{Context.User.Mention}** has kicked you from **{Context.Guild.Name}**";
            string reply = $"you have successfully kicked **{userToKick.Mention}**";

            if (string.IsNullOrWhiteSpace(reason))
            {
                message += $" for **{reason}**";
                reply += $" for **{reason}**";
            }

            await _moderationService.InformUserAsync(userToKick, message + ".");

            await userToKick.KickAsync(reason);

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Kick", Configuration.KickColor, reason, Context.User as IGuildUser, userToKick);

            await ReplyAsync(reply + ".");
        }
    }
}
