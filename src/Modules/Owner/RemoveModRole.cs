using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Owner
{
    public partial class Owner
    {
        [Command("RemoveModRole")]
        [Alias("removemoderatorrole")]
        [Summary("Remove a role from the list of moderators.")]
        [Remarks("@Moderator")]
        public async Task RemoveModRole([Summary("The role you want to remove from the list of mod roles")][Remainder] IRole modRole)
        {
            if (Context.DbGuild.ModRoles.Any(x => x.Name == modRole.Id.ToString()))
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ModRoles.Remove(modRole.Id.ToString()));

                await ReplyAsync($"You have successfully removed **{modRole.Mention}** from the list of moderators.");
            }
            else
            {
                await ReplyErrorAsync($"The role **{modRole.Mention}** is not a moderator role, and therefore cannot be removed.");
            }
        }
    }
}
