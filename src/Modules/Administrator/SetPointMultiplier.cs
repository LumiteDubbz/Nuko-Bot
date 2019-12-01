using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetPointMultiplier")]
        [Alias("setmultiplier", "setguildmultiplier", "setguildpointmultiplier", "setservermultiplier", "setserverpointmultiplier", "removemultiplier", "removepointmultiplier")]
        [Summary("Set the multiplier that all awarded points will be multiplied by until set to default")]
        public async Task SetPointMultiplier([Summary("The point multiplier.")][Remainder] double multiplier = 1)
        {
            if (multiplier > Configuration.MaximumMultiplier || multiplier < Configuration.MinimumMultiplier)
            {
                await ReplyErrorAsync($"you must set a multiplier between {Configuration.MinimumMultiplier}x and {Configuration.MaximumMultiplier}x.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.PointMultiplier = multiplier);

            if (multiplier > 1)
            {
                await ReplyAsync($"you have successfully set this server's point multiplier to {multiplier}x.");
                return;
            }

            await ReplyAsync("you have successfully set the point multiplier to default.");
        }
    }
}
