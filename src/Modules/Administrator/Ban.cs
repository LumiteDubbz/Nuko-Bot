using Discord;
using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("Ban")]
        [Alias("banish")]
        [Summary("Ban any user from being in the server.")]
        [Remarks("\"@John Doe#6634\" \"Sent NSFW content.\"")]
        public async Task Ban([Summary("The user to ban.")] IGuildUser userToBan, [Summary("The reason for banning the user.")][Remainder] string reason = null)
        {
            if (_moderationService.GetPermissionLevel(Context.DbGuild, userToBan) > 0)
            {
                await ReplyErrorAsync($"**{userToBan.Mention}** is a moderator and thus cannot be banned.");
                return;
            }

            string message = $"**{Context.User.Mention}** has banned you from **{Context.Guild.Name}**";
            string reply = $"You have successfully banned **{userToBan.Mention}**";

            if (!string.IsNullOrWhiteSpace(reason))
            {
                message += $" for **{reason}**";
                reply += $" for **{reason}**";
            }

            await _moderationService.InformUserAsync(userToBan, message + ".");

            await userToBan.BanAsync(0, reason);

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Ban", Configuration.KickColor, reason, Context.User as IGuildUser, userToBan);

            var dbUser = await _userRepository.GetUserAsync(userToBan.Id, userToBan.GuildId);

            if (!dbUser.HasBeenBanned) await _userRepository.ModifyUserAsync(userToBan, x => x.HasBeenBanned = true);

            await ReplyAsync(reply + ".");
        }
    }
}
