using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetAutomaticMuteTime")]
        [Alias("setwarningmutetime", "automaticmutetime", "warningmutetime")]
        [Summary("Set the time for mutes given by users exceeding the warn limit.")]
        [Remarks("24")]
        public async Task SetAutomaticMuteTime(double hours)
        {
            if (Context.DbGuild.WarnMuteLength == hours)
            {
                await ReplyErrorAsync($"this server's warning mute time is already set to **{hours}**.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.WarnMuteLength = hours);

            await ReplyAsync($"you have successfully set this server's automatic mute length to **{hours}**");
        }
    }
}
