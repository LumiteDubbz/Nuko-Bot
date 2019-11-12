using MongoDB.Driver;
using NukoBot.Database.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace NukoBot.Database.Repositories
{
    public sealed class RunRepository : BaseRepository<Run>
    {
        public RunRepository(IMongoCollection<Run> runs) : base(runs) { }

        public async Task AddRunAsync(DateTime time)
        {
            int runNumber = 1;

            var runs = await AllAsync(x => x.RunNumber > 0);

            if (runs.Any())
            {
                var query = runs.OrderByDescending(x => x.RunNumber).Take(1);

                runNumber = query.First().RunNumber + 1;
            }

            var run = new Run()
            {
                TimeCommenced = time,
                RunNumber = runNumber
            };

            await InsertAsync(run);
        }
    }
}