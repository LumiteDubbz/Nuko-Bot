using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.General
{
    public partial class General
    {
        [Command("SetNick")]
        [Alias("nick", "changenick", "updatenick")]
        [Summary("Changes the nickname of a specified user.")]
        [Remarks("@Michael#0146 A GOOD KID")]
        public async Task SetNick(IGuildUser user, [Remainder] string nick = null)
        {
            if (user.Id != Context.User.Id)
            {
                if (_moderationService.GetPermissionLevel(Context.DbGuild, user) > 0)
                {
                    await ReplyErrorAsync($"**{user.Mention}** is a moderator, so you cannot change their nickname.");
                    return;
                }
            }

            try
            {
                await user.ModifyAsync(x => x.Nickname = nick);

                string message = $"you have successfully set **{user.Mention}**'s nicknname to ";

                message += nick == null ? "nothing" : $"{nick}";

                await ReplyAsync(message + ".");
            }
            catch
            {
                await ReplyErrorAsync($"I cannot change **{user.Mention}**'s nickname.");
            }
        }
    }
}
