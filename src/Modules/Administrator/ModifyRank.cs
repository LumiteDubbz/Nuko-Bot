using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("ModifyRank")]
        [Alias("editrank", "editrankrole", "modifyrankrole")]
        [Summary("Modify the point requirement for any rank role.")]
        [Remarks("\"@Level 1\" 200")]
        public async Task ModifyRank(IRole role, double pointsRequired)
        {
            if (!Context.DbGuild.OverhaulEnabled)
            {
                await ReplyErrorAsync("all Overhaul related commands are disabled on this server.");
                return;
            }

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

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.RankRoles[role.Id.ToString()] = pointsRequired);

            await ReplyAsync($"you have successfully updated the **{role.Mention}** rank to have a point requirement of **{pointsRequired}**.");
        }
    }
}
