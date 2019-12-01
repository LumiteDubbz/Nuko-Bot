using Discord;
using Discord.Commands;
using NukoBot.Common;
using System;
using System.Threading.Tasks;

namespace NukoBot.Modules.Moderator
{
    public partial class Moderator
    {
        [Command("Mute")]
        [Alias("silence")]
        [Summary("Mute a user until they are manually unmuted.")]
        public async Task Mute([Summary("The user you want to mute.")] IGuildUser userToMute, [Summary("The reason for muting the user.")][Remainder] string reason = null)
        {
            var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

            if (mutedRole == null)
            {
                await ReplyErrorAsync("there is no muted role set for this server. Please use the ``SetMutedRole`` command to remedy this error.");
                return;
            }
            else if (_moderationService.GetPermissionLevel(Context.DbGuild, userToMute) > 0)
            {
                await ReplyErrorAsync($"{userToMute.Mention} is a moderator and thus cannot be muted.");
                return;
            }

            await userToMute.AddRoleAsync(mutedRole);

            await _muteRepository.InsertMuteAsync(userToMute, TimeSpan.FromDays(365));

            await ReplyAsync($"you have successfully muted {userToMute.Mention}.");

            string message = $"**{Context.User.Mention}** has permanently muted you in **{Context.Guild.Name}**";

            if (reason.Length > 0)
            {
                message += $" for **{reason}**";
            }

            await _moderationService.InformUserAsync(userToMute, message + ".");

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Mute", Configuration.MuteColor, reason, Context.User as IGuildUser, userToMute);
        }
    }
}
