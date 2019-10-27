using MongoDB.Driver;
using NukoBot.Common;
using NukoBot.Database.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Database.Repositories
{
    public sealed class PollRepository : BaseRepository<Poll>
    {
        public PollRepository(IMongoCollection<Poll> polls) : base(polls) { }

        public async Task<Poll> GetPollAsync(Context context, string name, ulong guildId)
        {
            var poll = await GetAsync(x => x.Name.ToLower() == name.ToLower() && x.GuildId == context.Guild.Id);

            return poll;
        }

        public async Task<Poll> CreatePollAsync(Context context, string name, string[] choices, TimeSpan? length = null)
        {
            if (await AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.GuildId == context.Guild.Id))
            {
                return null;
            }

            var createdPoll = new Poll(name, context.User.Id, context.Guild.Id, choices);

            if (length.HasValue)
            {
                createdPoll.Length = length.Value.TotalHours;
            }

            await InsertAsync(createdPoll);
            return createdPoll;
        }
    }
}
