using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Database.Repositories;
using NukoBot.Common.Preconditions.Command;
using NukoBot.Services;
using System;
using System.Threading.Tasks;

namespace NukoBot.Modules
{
    [Name("Administrator")]
    [Summary("Commands only allowed to be used by users with a role with a permission level of at least 2.")]
    [RequireAdministrator]
    public sealed class Administrator : ModuleBase<Context>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;
        private readonly GuildRepository _guildRepository;
        private readonly UserRepository _userRepository;

        public Administrator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
            _guildRepository = _serviceProvider.GetRequiredService<GuildRepository>();
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
        }

        [Command("setscreenshotchannel")]
        [Alias("updatescreenshotchannel")]
        [Summary("Set the channel for screenshots to be submitted to.")]
        public async Task SetScreenshotChannel([Remainder] ITextChannel screenshotChannel)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ScreenshotChannelId = screenshotChannel.Id);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the screenshot channel to {screenshotChannel.Mention}.");
        }

        [Command("setmutedrole")]
        [Alias("setmuterole")]
        [Summary("Set the role to be given to those who are muted.")]
        public async Task SetMutedRole([Remainder] IRole mutedRole)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.MutedRoleId = mutedRole.Id);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the muted role to {mutedRole.Mention}.");
        }

        [Command("setmodlogchannel")]
        [Alias("setlogchannel")]
        [Summary("Set the channel for all moderator actions to be logged to.")]
        public async Task SetModLogChannel([Remainder] ITextChannel modLogChannel)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ModLogChannelId = modLogChannel.Id);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the mod log channel to {modLogChannel.Mention}.");
        }

        [Command("setwelcomechannel")]
        [Alias("setwelcomemessagechannel")]
        [Summary("Set the channel for welcome messages to be sent to.")]
        public async Task SetWelcomeChannel([Remainder] ITextChannel welcomeChannel)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.WelcomeChannelId = welcomeChannel.Id);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the welcome channel to {welcomeChannel.Mention}.");
        }

        [Command("setwelcomemessage")]
        [Alias("setwelcomemsg")]
        [Summary("Set the message to sent when a user joins this server.")]
        public async Task SetWelcomeChannel([Remainder] string welcomeMessage)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.WelcomeMessage = welcomeMessage);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the welcome message to **{welcomeMessage}**");
        }

        [Command("setnewuserrole")]
        [Alias("setdefaultrole")]
        [Summary("Set the role to be given to all new users in the server.")]
        public async Task SetNewUserRole([Remainder] IRole newUserRole)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.NewUserRole = newUserRole.Id);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the new user role to {newUserRole.Mention}.");
        }

        [Command("award")]
        [Alias("awardpoints", "givepoints")]
        [Summary("Add points to a user which will also increase the glboal point counter.")]
        public async Task Award(int amountOfPoints, [Remainder] IGuildUser user)
        {
            var dbUser = await _userRepository.GetUserAsync(user.Id, Context.Guild.Id);

            if (dbUser == null)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"that user was found.");
                return;
            }

            await _userRepository.ModifyAsync(dbUser, x => x.Points += amountOfPoints);

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points += amountOfPoints);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully added **{amountOfPoints}** to {user.Mention}.");
        }

        [Command("deduct")]
        [Alias("deductpoints", "removepoints")]
        [Summary("Remove points from a user which will also decrease the global point counter.")]
        public async Task Deduct(int amountOfPoints, [Remainder] IGuildUser user)
        {
            var dbUser = await _userRepository.GetUserAsync(user.Id, Context.Guild.Id);

            if (dbUser == null)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"that user was found.");
                return;
            }

            await _userRepository.ModifyAsync(dbUser, x => x.Points -= amountOfPoints);

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.Points -= amountOfPoints);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully removed **{amountOfPoints}** from {user.Mention}.");
        }
    }
}
