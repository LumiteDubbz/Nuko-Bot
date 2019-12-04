using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.BotOwner
{
    public partial class BotOwner
    {
        [Command("SetGame")]
        [Alias("setstatus")]
        [Summary("Sets the current playing status for the bot")]
        [Remarks("say >help")]
        public async Task SetGame([Summary("The status you want to set.")][Remainder] string game = null)
        {
            if (game == null)
            {
                game = Configuration.Game;
            }

            await Context.Client.SetGameAsync(game);

            await ReplyAsync($"you have successfully set the game to **{game}**.");
        }
    }
}
