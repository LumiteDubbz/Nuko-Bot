using Discord;
using Discord.Commands;
using NukoBot.Common;
using System;
using System.Threading.Tasks;

namespace NukoBot.Modules.Moderator
{
    public partial class Moderator
    {
        [Command("custommute")]
        [Alias("customsilence")]
        [Summary("Mute a user for a set amount of time.")]
        public async Task CustomMuteAsync(double hours, IGuildUser userToMute, [Remainder] string reason = null)
        {
            if (hours < 1)
            {
                await ReplyErrorAsync("you cannot mute anyone for less than 1 hour.");

                return;
            }

            var time = hours == 1 ? "hour" : "hours";

            var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

            if (mutedRole == null)
            {
                await ReplyErrorAsync("there is no muted role set for this server. Please use the ``SetMutedRole`` command to remedy this error.");

                return;
            }

            await userToMute.AddRoleAsync(mutedRole);

            await _muteRepository.InsertMuteAsync(userToMute, TimeSpan.FromHours(hours));

            await ReplyAsync($"you have successfully muted {userToMute.Mention} for {hours} {time}.");

            string message = $"**{Context.User.Mention}** has muted you in **{Context.Guild.Name}** for **{hours}** {time}";

            if (reason.Length > 0)
            {
                message += $" for **{reason}**";
            }

            await _moderationService.InformUserAsync(userToMute, message + ".");

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Mute", Configuration.MuteColor, reason, Context.User as IGuildUser, userToMute);
        }
    }
}
