using Discord;
using NukoBot.Database.Models;
using NukoBot.Database.Repositories;
using System.Linq;

namespace NukoBot.Services
{
    public sealed class ModerationService
    {
        private readonly GuildRepository _guildRepo;

        public ModerationService(GuildRepository guildRepo)
        {
            _guildRepo = guildRepo;
        }

        public int GetPermissionLevel(Guild dbGuild, IGuildUser user)
        {
            if (user.Guild.OwnerId == user.Id)
            {
                return 3;
            }

            var permLevel = 0;

            if (dbGuild.ModRoles != 0)
            {
                foreach (var role in dbGuild.ModRoles.OrderBy(x => x.Value))
                {
                    if (user.Guild.GetRole(ulong.Parse(role.Name)) != null)
                    {
                        if (user.RoleIds.Any(x => x.ToString() == role.Name))
                        {
                            permLevel = role.Value.AsInt32;
                        }
                    }
                }
            }

            return user.GuildPermissions.Administrator && permLevel < 2 ? 2 : permLevel;
        }
    }
}
