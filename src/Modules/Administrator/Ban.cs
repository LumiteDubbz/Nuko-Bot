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
        [Remarks("\"@John Doe\" \"Sent NSFW content.\"")]
        public async Task Ban([Summary("The user to ban.")] IGuildUser userToBan, [Summary("The reason for banning the user.")][Remainder] string reason = null)
        {
            if (_moderationService.GetPermissionLevel(Context.DbGuild, userToBan) > 0)
            {
                await ReplyErrorAsync($"{userToBan.Mention} is a moderator and thus cannot be banned.");
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

            await ReplyAsync(reply + ".");
        }
    }
}
