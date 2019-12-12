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
                    if (warning.CreatedAt.AddDays(Context.DbGuild.DaysUntilWarningExpires) <= DateTime.Now && !warning.Expired)
                    {
                        await _userRepository.ModifyAsync(dbUser, x => x.Warnings.Single(x => x.CreatedAt == warning.CreatedAt).Expired = true);
                    }

                    var isExpired = warning.Expired ? "expired" : "not expired";

                    warnings += $"Warned for **{warning.Reason}** by **{Context.Client.GetUser(warning.ModeratorId).Mention}** on **{warning.CreatedAt}** ({isExpired}).\n";
                }

                message += "\n" + warnings.Remove(warnings.Length - 1);
            }

            var muteStatus = await _muteRepository.IsMutedAsync(user.Id, user.GuildId) ? "\nIs currently muted." : dbUser.HasBeenMuted ? "\nHas been muted in the past." : "\nNot currently muted.";
            var kickStatus = dbUser.HasBeenKicked ? "\nHas been kicked in the past." : "\nHas never been kicked.";
            var banStatus = Context.Guild.GetBansAsync().Result.Any(x => x.User.Id == user.Id) ? "\nIs currently banned." : dbUser.HasBeenBanned ? "\nHas been banned in the past." : "\nHas never been banned.";

            message += muteStatus + kickStatus + banStatus;

            await SendAsync(message);
        }
    }
}
