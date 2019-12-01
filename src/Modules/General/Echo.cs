using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.General
{
    public partial class General
    {
        [Command("Echo")]
        [Alias("say", "embed")]
        [Summary("Repeat the provided text in an embedded message.")]
        public async Task Echo([Summary("The text you want the bot to embed.")][Remainder] string message)
        {
            await SendAsync(message);
        }
    }
}
