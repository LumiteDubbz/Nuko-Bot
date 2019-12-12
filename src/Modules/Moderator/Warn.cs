using Discord;
using Discord.Commands;
using NukoBot.Common;
using NukoBot.Common.Structures;
using System;
using System.Threading.Tasks;

namespace NukoBot.Modules.Moderator
{
    public partial class Moderator
    {
        [Command("Warn")]
        [Alias("issuewarn", "issuewarning", "givewarn", "givewarning")]
        [Summary("Warns a user for breaking a rule in DMs and logs the warning.")]
        [Remarks("@CJ#2004 Didn't follow the damn train.")]
        public async Task Warn(IGuildUser user, [Remainder] string warning)
        {
            if (_moderationService.GetPermissionLevel(Context.DbGuild, user) > 0)
            {
                await ReplyErrorAsync($"**{user.Mention}** is a moderator and thus cannot be warned.");
                return;
            }

            await _userRepository.ModifyUserAsync(user, x => x.Warnings.Add(new Warning() { CreatedAt = DateTime.Now, ModeratorId = Context.User.Id, Reason = warning, Expired = false }));

            await _moderationService.InformUserAsync(user, $"**{Context.User.Mention}** has warned you for **{warning}** in **{Context.Guild.Name}**.");

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Warn", Configuration.WarnColor, warning, (IGuildUser)Context.User, user);

            await ReplyAsync($"you have successfully warned **{user.Mention}** for **{warning}**.");
        }
    }
}
