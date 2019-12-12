using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetWarnExpiration")]
        [Alias("setwarnlength", "setwarnduration", "setwarncooldown", "setwarningexpiration")]
        [Summary("Sets the amount of days until warnings expire.")]
        [Remarks("30")]
        public async Task SetWarnExpiration(int days)
        {
            if (Context.DbGuild.DaysUntilWarningExpires == days)
            {
                await ReplyErrorAsync($"this server's warning expiration is already set to **{days}**.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.DaysUntilWarningExpires = days);

            await ReplyAsync($"you have successfully set this server's warning expiration to **{days}**.");
        }
    }
}
