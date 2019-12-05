using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("ToggleOverhaul")]
        [Alias("disableoverhaul", "enableoverhaul")]
        [Summary("Toggle the ability for all Overhaul-related commands to be ran in this server.")]
        public async Task DisableOverhaul()
        {
            string message;

            if (Context.DbGuild.OverhaulEnabled)
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.OverhaulEnabled = false);

                message = "you have successfully disabled all Overhaul commands in this server.";
            }
            else
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.OverhaulEnabled = true);

                message = "you have successfully enabled all Overhaul commands in this server.";
            }

            await ReplyAsync(message);
        }
    }
}
