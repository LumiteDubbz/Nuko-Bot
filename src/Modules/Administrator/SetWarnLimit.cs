using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetWarnLimit")]
        [Alias("warnlimit", "warnactionlimit", "setwarnactionlimit")]
        [Summary("Set the amount of warnings a user may recieve before they are acted against.")]
        [Remarks("2")]
        public async Task WarnLimit(int limit)
        {
            if (limit == Context.DbGuild.MaxmimumWarningsBeforeAction)
            {
                await ReplyErrorAsync($"this server's warning limit is already **{limit}**.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.MaxmimumWarningsBeforeAction = limit);

            await ReplyAsync($"you have successfully set this server's warning limit to **{limit}**.");
        }
    }
}
