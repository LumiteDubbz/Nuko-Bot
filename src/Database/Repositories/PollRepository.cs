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

        public async Task<Poll> GetPollAsync(int index, ulong guildId)
        {
            var polls = await AllAsync(x => x.GuildId == guildId);

            polls = polls.OrderBy(x => x.CreatedAt).ToList();

            try
            {
                return polls[index - 1];
            }
            catch (ArgumentOutOfRangeException)
            {
                return null;
            }
        }

        public async Task<Poll> CreatePollAsync(ulong userId, ulong guildId, string name, string[] choices, TimeSpan? length = null)
        {
            if (await AnyAsync(x => x.Name.ToLower() == name.ToLower() && x.GuildId == guildId))
            {
                return null;
            }

            var createdPoll = new Poll(name, userId, guildId, choices);

            if (length.HasValue)
            {
                createdPoll.Length = length.Value.TotalHours;
            }

            await InsertAsync(createdPoll);

            return createdPoll;
        }

        public async Task RemovePollAsync(int index, ulong guildId)
        {
            var polls = (await AllAsync(x => x.GuildId == guildId)).OrderBy(x => x.CreatedAt).ToList();

            var poll = polls[index - 1];

            await DeleteAsync(x => x.Id == poll.Id);
        }
    }
}