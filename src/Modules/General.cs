using Discord;
using Discord.Commands;
using Discord.WebSocket;
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
    [Name("General")]
    [Summary("General commands not directly related to the core functionality of the bot.")]
    [RequireContext(ContextType.Guild)]
    public sealed class General : ModuleBase<Context>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly Text _text;
        private readonly UserRepository _userRepository;
        private readonly PollRepository _pollRepository;

        public General(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _text = _serviceProvider.GetRequiredService<Text>();
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
            _pollRepository = _serviceProvider.GetRequiredService<PollRepository>();
        }

        [Command("submit")]
        [Alias("submit-screenshot")]
        [Summary("Submit a screenshot of your game to the staff for points.")]
        [RequireAttachedImage]
        public async Task Submit()
        {
            await _text.SendScreenshotAsync(_client.GetChannel(Context.DbGuild.ScreenshotChannelId) as IMessageChannel, $"**{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}** has submitted this screenshot.", Context.Message.Attachments.ElementAt(0));
        }

        [Command("points")]
        [Alias("pointcount")]
        [Summary("View the amount of points you or a mentioned user has.")]
        public async Task Points([Remainder] IUser user = null)
        {
            if (user != null)
            {
                var dbUser = await _userRepository.GetUserAsync(user.Id, Context.Guild.Id);

                await _text.ReplyAsync(Context.User, Context.Channel, $"{user.Mention} has **{dbUser.Points}** points, contributing to this guild's total of **{Context.DbGuild.Points}** points.");
                return;
            }

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have **{Context.DbUser.Points}** points, contributing to this guild's total of **{Context.DbGuild.Points}** points.");
        }

        [Command("vote")]
        [Alias("votepoll")]
        [Summary("Vote on any active poll")]
        public async Task Vote(int pollIndex, int choiceIndex)
        {
            var poll = await _pollRepository.GetPollAsync(pollIndex, Context.Guild.Id);

            if (poll != null)
            {
                if (poll.VotesDocument.Any(x => x.Name == Context.User.Id.ToString()))
                {
                    await _text.ReplyErrorAsync(Context.User, Context.Channel, "you have already voted on this poll.");

                    return;
                }

                string choice = null;

                try
                {
                    choice = poll.Choices[choiceIndex - 1];
                }
                catch (IndexOutOfRangeException)
                {
                    await _text.ReplyErrorAsync(Context.User, Context.Channel, "no vote with that index (ID) was found.");
                }

                await _pollRepository.ModifyAsync(poll, x => x.VotesDocument.Add(Context.User.Id.ToString(), choice));

                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully voted on **{poll.Name}**.");

                return;
            }

            await _text.ReplyErrorAsync(Context.User, Context.Channel, "no poll with that index (ID) was found.");
        }
    }
}
