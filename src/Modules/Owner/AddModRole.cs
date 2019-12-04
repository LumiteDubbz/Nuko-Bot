using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Owner
{
    public partial class Owner
    {
        [Command("AddModRole")]
        [Alias("addmoderatorrole", "modifymodrole", "modifymoderatorrole")]
        [Summary("Add a role to the list of moderators.")]
        [Remarks("@Admin 2")]
        public async Task AddModRole([Summary("The role you wish to assign a permission level.")] IRole modRole, [Summary("The permission level you want to give the role.")] int permissionLevel = 1)
        {
            if (permissionLevel < 1 || permissionLevel > 3)
            {
                await ReplyErrorAsync("The permission level can only be 1, 2 or 3, corresponding to Moderator, Administrator and Owner respectively.");
                return;
            }

            if (Context.DbGuild.ModRoles.ElementCount == 0)
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ModRoles.Add(modRole.Id.ToString(), permissionLevel));
            }
            else
            {
                if (Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
                {
                    await ReplyAsync($"you have successfully updated the moderator role {modRole.Mention} to have a permission level of {permissionLevel}.");
                }

                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ModRoles.Add(modRole.Id.ToString(), permissionLevel));
            }

            await ReplyAsync($"you have successfully added {modRole.Mention} as a mod role with the permission level of {permissionLevel}.");
        }
    }
}
