using Discord.Commands;
using NukoBot.Common;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Moderator
{
    public partial class Moderator
    {
        [Command("CreatePoll")]
        [Alias("makepoll", "addpoll")]
        [Summary("Create a poll for people to vote on.")]
        public async Task CreatePoll([Summary("The question of the poll.")] string name, [Summary("The chocies people can vote for, separated by `~`s")] string choices, [Summary("The number of hours the poll should last.")][Remainder] double hoursToLast = 1)
        {
            var choicesArray = choices.Split('~');

            if (choicesArray.Distinct().Count() != choicesArray.Length)
            {
                await ReplyErrorAsync("you cannot make a poll with vote options that are the same.");
                return;
            }

            var poll = await _pollRepository.CreatePollAsync(Context.User.Id, Context.Guild.Id, name, choicesArray, TimeSpan.FromHours(hoursToLast));

            if (poll == null)
            {
                await ReplyErrorAsync("a poll with that name already exists.");
            }

            var polls = await _pollRepository.AllAsync(x => x.GuildId == Context.Guild.Id);

            var pollCount = polls.OrderBy(x => x.CreatedAt).ToList().Count();

            string message = $"you have made a poll with the following fields:\n\nName:{poll.Name},\nLength: {poll.Length}h,\nIndex (ID): {pollCount},\nChoices: ";

            foreach (var choice in choicesArray)
            {
                message += choice + ", ";
            }

            await ReplyAsync($"{message.Remove(message.Length - 2)}.");
        }
    }
}
