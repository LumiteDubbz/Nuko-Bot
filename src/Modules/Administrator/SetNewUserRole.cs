using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetNewUserRole")]
        [Alias("setdefaultrole")]
        [Summary("Set the role to be given to all new users in the server.")]
        public async Task SetNewUserRole([Summary("The role to be given to new users.")][Remainder] IRole newUserRole = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.NewUserRole = newUserRole.Id);

            if (newUserRole != null)
            {
                await ReplyAsync($"you have successfully set the new user role to {newUserRole.Mention}.");
                return;
            }

            await ReplyAsync("you have successfully removed the new user role.");
        }
    }
}
