using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.System
{
    public partial class System
    {
        [Command("Support")]
        [Alias("report-bug", "reportbug")]
        [Summary("Displays the invitation link to the support server.")]
        public async Task Support()
        {
            await ReplyAsync("for bot support, selfhosting support or feature requests, join the support server [here](" + Configuration.SupportServerLink + ").");
        }
    }
}
