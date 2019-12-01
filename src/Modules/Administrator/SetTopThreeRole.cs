using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetTop3Role")]
        [Alias("settoprole", "setlbrole", "setleaderboardrole", "settop3")]
        [Summary("Set the role to be given to the 3 users with the most points.")]
        public async Task SetTopThreeRole([Summary("The role to be given.")][Remainder] IRole topThreeRole)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.TopThreeRole = topThreeRole.Id);

            if (topThreeRole != null)
            {
                await ReplyAsync($"you have successfully set the top 3 role to {topThreeRole.Mention}.");
                return;
            }

            await ReplyAsync("you have successfully removed the top 3 role.");
        }
    }
}
