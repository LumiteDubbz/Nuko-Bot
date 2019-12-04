using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("RemoveRank")]
        [Alias("deleterank", "deleterankrole")]
        [Summary("Remove a rank role.")]
        [Remarks("@Level 3")]
        public async Task RemoveRank([Remainder] IRole role)
        {
            if (Context.DbGuild.RankRoles.ElementCount == 0)
            {
                await ReplyErrorAsync("this server has no rank roles.");
                return;
            }

            if (!Context.DbGuild.RankRoles.Any(x => x.Name == role.Id.ToString()))
            {
                await ReplyErrorAsync($"**{role.Mention}** is not a rank role.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.RankRoles.Remove(role.Id.ToString()));

            await ReplyAsync($"you have successfully removed **{role.Mention}** from this server's rank roles.");
        }
    }
}
