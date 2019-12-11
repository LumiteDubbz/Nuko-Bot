using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NukoBot.Common.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.General
{
    public partial class General
    {
        [Command("RoleInfo")]
        [Alias("getroleinfo", "roleinformation", "getroleinformation", "role")]
        [Summary("Shows information about a given role.")]
        [Remarks("@Member")]
        public async Task RoleInfo(IRole role = null)
        {
            role ??= (Context.User as SocketGuildUser).Roles.OrderByDescending(x => x.Position).FirstOrDefault();

            if (role == null)
            {
                await ReplyErrorAsync("you need to provide or have at least one role.");
                return;
            }

            var permissions = "";

            if (!role.Permissions.Administrator)
            {
                foreach (var permission in role.Permissions.ToList())
                {
                    permissions += $"{permission},\n";
                }
            }
            else
            {
                permissions += "Administrator,\n";
            }

            await SendAsync($"**@{role.Name} ({role.Id})\n\nCreated at:** {role.CreatedAt.Date + role.CreatedAt.TimeOfDay}\n**Colour:** {role.Color}\n**Mentionable:** {role.IsMentionable.ToString().WithUppercaseFirstCharacter()}\n**Position:** {role.Position}\n**Permissions:**\n{permissions.Remove(permissions.Length - 2)}.", color: role.Color.ToString() != "#0" ? role.Color : (Color?)null);
        }
    }
}
