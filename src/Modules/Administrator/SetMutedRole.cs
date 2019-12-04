using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetMutedRole")]
        [Alias("setmuterole", "removemutedrole", "removemuterole")]
        [Summary("Set the role to be given to those who are muted.")]
        [Remarks("@Muted")]
        public async Task SetMutedRole([Summary("The role you want to set as the muted role.")][Remainder] IRole mutedRole = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.MutedRoleId = mutedRole.Id);

            if (mutedRole != null)
            {
                await ReplyAsync($"you have successfully set the muted role to {mutedRole.Mention}.");
                return;
            }

            await ReplyAsync("you have successfully removed the muted role.");
        }
    }
}
