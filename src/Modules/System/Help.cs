using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.System
{
    public partial class System
    {
        [Command("Help")]
        [Alias("info")]
        [Summary("View the basic information regarding this bot.")]
        public async Task Help()
        {
            var userDm = await Context.User.GetOrCreateDMChannelAsync();

            await DmAsync(Context.User, Configuration.HelpMessage);

            if (Context.Channel != userDm)
            {
                await ReplyAsync("please check your DMs.");
            }
        }
    }
}
