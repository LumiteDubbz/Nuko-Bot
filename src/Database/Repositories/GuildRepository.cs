using MongoDB.Driver;
using NukoBot.Database.Models;
using System.Threading.Tasks;

namespace NukoBot.Database.Repositories
{
    public sealed class GuildRepository : BaseRepository<Guild>
    {
        public GuildRepository(IMongoCollection<Guild> guilds) : base(guilds) { }

        public async Task<Guild> GetGuildAsync(ulong guildId)
        {
            var dbGuild = await GetAsync(x => x.GuildId == guildId);

            if (dbGuild == default)
            {
                var createdGuild = new Guild(guildId);

                await InsertAsync(createdGuild);

                return createdGuild;
            }

            return dbGuild;
        }
    }
}
