using Discord;
using Discord.Commands;
using NukoBot.Common;
using NukoBot.Database.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Overhaul
{
    public partial class Overhaul
    {
        [Command("Leaderboard")]
        [Alias("top", "topusers", "lb")]
        [Summary("View 3 users with the most points in the server.")]
        public async Task Leaderboard()
        {
            if (!Context.DbGuild.OverhaulEnabled)
            {
                await ReplyErrorAsync("all Overhaul related commands are disabled on this server.");
                return;
            }

            var users = (await _userRepository.AllAsync(x => x.GuildId == Context.Guild.Id)).OrderByDescending(x => x.Points);
            var message = string.Empty;

            int position = 1;

            if (users.Count() == 0)
            {
                await ReplyErrorAsync("there are no users on the learderboard yet.");
                return;
            }

            var guildInterface = Context.Guild as IGuild;

            foreach (User dbUser in users)
            {
                var user = await guildInterface.GetUserAsync(dbUser.UserId);

                if (user == null)
                {
                    continue;
                }

                message += $"**{position}**. {user.Mention}: {dbUser.Points} points\n";

                if (position >= Configuration.MaximumLeaderboardPosition)
                {
                    break;
                }

                position++;
            }

            await SendAsync(message);
        }
    }
}
