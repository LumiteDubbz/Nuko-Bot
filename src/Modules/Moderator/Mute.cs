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
        [Remarks("@Ass_Ass#2212 Spammed chat")]
        public async Task Mute([Summary("The user you want to mute.")] IGuildUser userToMute, [Summary("The reason for muting the user.")][Remainder] string reason = null)
        {
            var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

            if (mutedRole == null)
            {
                await ReplyErrorAsync($"there is no muted role set for this server. Please use the `{Configuration.Prefix}SetMutedRole` command to remedy this error.");
                return;
            }
            else if (_moderationService.GetPermissionLevel(Context.DbGuild, userToMute) > 0)
            {
                await ReplyErrorAsync($"**{userToMute.Mention}** is a moderator and thus cannot be muted.");
                return;
            }

            await userToMute.AddRoleAsync(mutedRole);

            await _muteRepository.InsertMuteAsync(userToMute, TimeSpan.FromDays(365));

            string message = $"**{Context.User.Mention}** has permanently muted you in **{Context.Guild.Name}**";

            if (!string.IsNullOrWhiteSpace(reason))
            {
                message += $" for **{reason}**";
            }

            await _moderationService.InformUserAsync(userToMute, message + ".");

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Mute", Configuration.MuteColor, reason, Context.User as IGuildUser, userToMute);

            var dbUser = await _userRepository.GetUserAsync(userToMute.Id, userToMute.GuildId);

            if (!dbUser.HasBeenMuted) await _userRepository.ModifyUserAsync(userToMute, x => x.HasBeenMuted = true);

            await ReplyAsync($"you have successfully muted **{userToMute.Mention}**.");
        }
    }
}
