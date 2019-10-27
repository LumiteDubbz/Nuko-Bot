using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Preconditions.Command;
using NukoBot.Database.Repositories;
using NukoBot.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules
{
    [Name("Moderator")]
    [Summary("Commands only allowed to be used by users with a role with a permission level of at least 1.")]
    [RequireModerator]
    public sealed class Moderator : ModuleBase<Context>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;
        private readonly GuildRepository _guildRepository;
        private readonly UserRepository _userRepository;
        private readonly PollRepository _pollRepository;

        public Moderator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
            _guildRepository = _serviceProvider.GetRequiredService<GuildRepository>();
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
            _pollRepository = _serviceProvider.GetRequiredService<PollRepository>();
        }

        [Command("createpoll")]
        [Alias("makepoll", "addpoll")]
        [Summary("Create a poll for people to vote on.")]
        public async Task CreatePoll(string name, string choices, [Remainder] double hoursToLast = 1)
        {
            var choicesArray = choices.Split('~');

            if (choicesArray.Distinct().Count() != choicesArray.Length)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "you cannot make a poll with vote options that are the same.");
                return;
            }

            var poll = await _pollRepository.CreatePollAsync(Context, name, choicesArray, TimeSpan.FromHours(hoursToLast));

            if (poll == null)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "a poll with that name already exists.");
            }

            string message = $"you have made a poll with the following fields:\n\nName:{poll.Name},\nLength: {poll.Length}h,\nChoices: ";

            foreach (var choice in choicesArray)
            {
                message += choice + ", ";
            }

            await _text.ReplyAsync(Context.User, Context.Channel, $"{message.Remove(message.Length - 2)}.");
        }

        [Command("deletepoll")]
        [Alias("removepoll", "destroypoll")]
        [Summary("Delete a poll.")]
        public async Task DeletePoll(string name)
        {
            var poll = await _pollRepository.GetPollAsync(Context, name, Context.Guild.Id);

            if (poll != null)
            {
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

                    message+= $"{x + 1}. {choice}: {votes[choice]} Votes ({percentage.ToString("P")})\n";
                }

                var user = Context.Client.GetUser(poll.CreatorId);

                var userDm = await user.GetOrCreateDMChannelAsync();

                await _text.SendAsync(userDm, $"These are the results from your poll **{poll.Name}**\n{message}");

                await _pollRepository.DeleteAsync(poll);

                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully deleted the poll **{poll.Name}** and the results have been sent to the poll creator in their DMs.");

                return;
            }

            await _text.ReplyErrorAsync(Context.User, Context.Channel, "no such poll with that name was found.");
        }
    }
}
