using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Moderator
{
    public partial class Moderator
    {
        [Command("DeletePoll")]
        [Alias("removepoll", "destroypoll")]
        [Summary("Delete a poll.")]
        [Remarks("1")]
        public async Task DeletePoll([Summary("The number corresponding to the poll you want to delete.")] int index)
        {
            var poll = await _pollRepository.GetPollAsync(index, Context.Guild.Id);

            if (poll != null)
            {
                await _pollRepository.RemovePollAsync(index, Context.Guild.Id);

                var message = string.Empty;

                var votes = poll.Votes();

                for (int x = 0; x < poll.Choices.Length; x++)
                {
                    var choice = poll.Choices[x];

                    var percentage = (votes[choice] / (double)poll.VotesDocument.ElementCount);

                    if (double.IsNaN(percentage))
                    {
                        percentage = 0;
                    }

                    message += $"{x + 1}. {choice}: {votes[choice]} Votes ({percentage.ToString("P")})\n";
                }

                var user = Context.Client.GetUser(poll.CreatorId);

                await DmAsync(user, $"These are the results from your poll **{poll.Name}**\n{message}");

                await ReplyAsync($"you have successfully deleted the poll **{poll.Name}** and the results have been sent to the poll creator in their DMs.");
                return;
            }

            await ReplyErrorAsync("no poll with that index (ID) was found.");
        }
    }
}
