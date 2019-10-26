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

        public General(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _text = _serviceProvider.GetRequiredService<Text>();
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
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
    }
}
