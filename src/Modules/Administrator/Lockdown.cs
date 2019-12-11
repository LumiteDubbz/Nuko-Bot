using Discord;
using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("Lockdown")]
        [Alias("lockdownserver", "close", "closeserver", "togglelockdown", "togglelock", "lock", "unlock")]
        [Summary("Immediately ban all new users as soon as they join.")]
        [Remarks("Preventing raid")]
        public async Task Lockdown([Remainder] string reason = null)
        {
            var message = "you have successfully ";

            reason ??= "";

            string action;

            if (Context.DbGuild.LockedDown)
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.LockedDown = false);

                action = "**unlocked**";

                await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Unlock", Configuration.UnmuteColor, reason, (IGuildUser) Context.User, null);
            }
            else
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.LockedDown = true);

                action = "**locked**";

                await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Lock", Configuration.BanColor, reason, (IGuildUser) Context.User, null);
            }

            message += $"{action} this server";

            if (!string.IsNullOrWhiteSpace(reason))
            {
                message += $" for **{reason}**";
            }

            await ReplyAsync(message + ".");
        }
    }
}
