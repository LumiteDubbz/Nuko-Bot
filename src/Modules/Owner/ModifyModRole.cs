using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Owner
{
    public partial class Owner
    {
        [Command("ModifyModRole")]
        [Alias("editmodrole", "editmoderatorrole", "modifymoderatorrole")]
        [Summary("Modify the permission level of a given mod role.")]
        [Remarks("@Moderator 1")]
        public async Task ModifyModRole(IRole modRole, int permissionLevel)
        {
            if (permissionLevel < 1 || permissionLevel > 3)
            {
                await ReplyErrorAsync("the permission level can only be 1, 2 or 3, corresponding to Moderator, Administrator and Owner respectively.");
                return;
            }

            if (Context.DbGuild.ModRoles.ElementCount == 0)
            {
                await ReplyErrorAsync("this server has no moderator roles.");
                return;
            }
            else if (!Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
            {
                await ReplyErrorAsync($"the role **{modRole.Mention}** is not a mod role.");
                return;
            }
            else if (Context.DbGuild.ModRoles.First(x => x.Name == modRole.Id.ToString()).Value == permissionLevel)
            {
                await ReplyErrorAsync($"that role already has a permission level of **{permissionLevel}**.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ModRoles[Context.DbGuild.ModRoles.IndexOfName(modRole.Id.ToString())] = permissionLevel);

            await ReplyAsync($"you have successfully modified the **{modRole.Mention}** role to have a permission level of **{permissionLevel}**.");
        }
    }
}
