using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("AddRank")]
        [Alias("createrank", "addrankrole")]
        [Summary("Add a rank role that users are awarded upon receiving the neccesary amount of points.")]
        [Remarks("@God#1220 5000")]
        public async Task AddRank(IRole role, double pointsRequired)
        {
            if (!Context.DbGuild.OverhaulEnabled)
            {
                await ReplyErrorAsync("all Overhaul related commands are disabled on this server.");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Roles.OrderByDescending(x => x.Position).First().Position)
            {
                await ReplyErrorAsync($"my role must be higher in the role order than **{role.Mention}**.");
                return;
            }

            if (Context.DbGuild.RankRoles == null || Context.DbGuild.RankRoles.ElementCount == 0)
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.RankRoles.Add(role.Id.ToString(), pointsRequired));
            }
            else
            {
                if (Context.DbGuild.RankRoles.Any(x => x.Name == role.Id.ToString()))
                {
                    await ReplyErrorAsync($"**{role.Mention}** is already a rank role.");
                    return;
                }

                if (Context.DbGuild.RankRoles.Any(x => (int)x.Value.AsDouble == (int)pointsRequired))
                {
                    await ReplyErrorAsync("a rank with that point requirement already exists.");
                    return;
                }

                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.RankRoles.Add(role.Id.ToString(), pointsRequired));
            }

            await ReplyAsync($"you have successfully added **{role.Mention}** as a rank role with a point requirement of **{pointsRequired}**.");
        }
    }
}
