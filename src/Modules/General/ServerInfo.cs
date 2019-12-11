using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.General
{
    public partial class General
    {
        [Command("ServerInfo")]
        [Alias("serverinformation", "guildinfo", "guildinformation")]
        [Summary("View information about the server this command is ran in.")]
        public async Task ServerInfo()
        {
            await SendImageAsync($"**{Context.Guild.Name} ({Context.Guild.Id})\n\nCreated at:** {Context.Guild.CreatedAt.DateTime + Context.Guild.CreatedAt.TimeOfDay}\n**Member count:** {Context.Guild.MemberCount}\n**Owner:** {Context.Guild.Owner.Mention}", Context.Guild.SplashUrl);
        }
    }
}
