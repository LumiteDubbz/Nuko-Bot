using Discord;
using Discord.Commands;
using NukoBot.Common.Structures;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Moderator
{
    public partial class Moderator
    {
        [Command("Report")]
        [Alias("viewwarnings", "checkwarnings", "seewarnings", "warnings")]
        [Summary("View a report on any given user, including their warnings.")]
        [Remarks("@Yuu#2017")]
        public async Task Report(IGuildUser user)
        {
            var message = $"**Report on {user.Mention}:**";
            var dbUser = await _userRepository.GetUserAsync(user.Id, user.GuildId);

            if (dbUser.Warnings.Any())
            {
                var warnings = "";

                foreach (var warning in dbUser.Warnings)
                {
                    if (warning.CreatedAt.AddDays(Context.DbGuild.DaysUntilWarningExpires) >= DateTime.Now)
                    {
                        var dbWarning = Context.DbUser.Warnings.Single(x => x.CreatedAt == warning.CreatedAt);
                        await _userRepository.ModifyUserAsync(user, x => dbWarning.Expired = true);
                    }

                    var isExpired = warning.Expired ? "expired" : "not expired";

                    warnings += $"Warned for **{warning.Reason}** by **{Context.Client.GetUser(warning.ModeratorId).Mention}** on **{warning.CreatedAt}** ({isExpired}).\n";
                }

                message += "\n" + warnings.Remove(warnings.Length - 1);
            }

            var isMuted = await _muteRepository.IsMutedAsync(user.Id, user.GuildId) ? "\nCurrently muted." : "\nNot currently muted.";
            var hasBeenMuted = Context.DbUser.HasBeenMuted ? "\nHas been muted in the past." : "\nHas never been muted.";
            var hasBeenKicked = Context.DbUser.HasBeenKicked ? "\nHas been kicked in the past." : "\nHas never been kicked.";
            var hasBeenBanned = Context.DbUser.HasBeenBanned ? "\nHas been banned in the past." : "\nHas never been banned.";

            message += isMuted + hasBeenMuted + hasBeenKicked + hasBeenBanned;

            await SendAsync(message);
        }
    }
}
