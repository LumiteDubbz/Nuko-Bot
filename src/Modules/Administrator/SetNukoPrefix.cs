using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetNukoPrefix")]
        [Alias("changenukoprefix", "updatenukoprefix", "setcustomnukoprefix", "nukoprefix")]
        [Summary("Set a custom command prefix for the server.")]
        [Remarks("-")]
        public async Task SetNukoPrefix(string prefix = null)
        {
            if (Context.DbGuild.Prefix == prefix)
            {
                await ReplyErrorAsync("that prefix is already set.");
                return;
            }

            prefix ??= Configuration.Prefix;

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Prefix = prefix);

            await ReplyAsync($"you have successfully set this guild's prefix to **{prefix}**.");
        }
    }
}
