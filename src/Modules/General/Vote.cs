using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.General
{
    public partial class General
    {
        [Command("Vote")]
        [Alias("votepoll")]
        [Summary("Vote on any active poll")]
        [Remarks("1 2")]
        public async Task Vote([Summary("The number corresponding to the poll you want to delete.")] int pollIndex, [Summary("The number corresponding to you choice you want to vote for.")] int choiceIndex)
        {
            var poll = await _pollRepository.GetPollAsync(pollIndex, Context.Guild.Id);

            if (poll != null)
            {
                if (poll.VotesDocument.Any(x => x.Name == Context.User.Id.ToString()))
                {
                    await ReplyErrorAsync("you have already voted on this poll.");
                    return;
                }

                string choice = null;

                try
                {
                    choice = poll.Choices[choiceIndex - 1];
                }
                catch (IndexOutOfRangeException)
                {
                    await ReplyErrorAsync("no vote with that index (ID) was found.");
                }

                await _pollRepository.ModifyAsync(poll, x => x.VotesDocument.Add(Context.User.Id.ToString(), choice));

                await ReplyAsync($"you have successfully voted on **{poll.Name}**.");
                return;
            }

            await ReplyErrorAsync("no poll with that index (ID) was found.");
        }
    }
}
