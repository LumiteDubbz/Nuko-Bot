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
    [Name("Moderator")]
    [Summary("Commands only allowed to be used by users with a role with a permission level of at least 1.")]
    [RequireModerator]
    public sealed class Moderator : ModuleBase<Context>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;
        private readonly PollRepository _pollRepository;
        private readonly MuteRepository _muteRepository;
        private readonly ModerationService _moderationService;

        public Moderator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
            _pollRepository = _serviceProvider.GetRequiredService<PollRepository>();
            _muteRepository = _serviceProvider.GetRequiredService<MuteRepository>();
            _moderationService = _serviceProvider.GetRequiredService<ModerationService>();
        }

        [Command("CreatePoll")]
        [Alias("makepoll", "addpoll")]
        [Summary("Create a poll for people to vote on.")]
        public async Task CreatePoll([Summary("The question of the poll.")] string name, [Summary("The chocies people can vote for, separated by `~`s")] string choices, [Summary("The number of hours the poll should last.")][Remainder] double hoursToLast = 1)
        {
            var choicesArray = choices.Split('~');

            if (choicesArray.Distinct().Count() != choicesArray.Length)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "you cannot make a poll with vote options that are the same.");
                return;
            }

            var poll = await _pollRepository.CreatePollAsync(Context.User.Id, Context.Guild.Id, name, choicesArray, TimeSpan.FromHours(hoursToLast));

            if (poll == null)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "a poll with that name already exists.");
            }

            var polls = await _pollRepository.AllAsync(x => x.GuildId == Context.Guild.Id);

            var pollCount = polls.OrderBy(x => x.CreatedAt).ToList().Count();

            string message = $"you have made a poll with the following fields:\n\nName:{poll.Name},\nLength: {poll.Length}h,\nIndex (ID): {pollCount},\nChoices: ";

            foreach (var choice in choicesArray)
            {
                message += choice + ", ";
            }

            await _text.ReplyAsync(Context.User, Context.Channel, $"{message.Remove(message.Length - 2)}.");
        }

        [Command("DeletePoll")]
        [Alias("removepoll", "destroypoll")]
        [Summary("Delete a poll.")]
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

                await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully deleted the poll **{poll.Name}** and the results have been sent to the poll creator in their DMs.");

                await _moderationService.InformUserAsync(user, $"These are the results from your poll **{poll.Name}**\n{message}");
                return;
            }

            await _text.ReplyErrorAsync(Context.User, Context.Channel, "no poll with that index (ID) was found.");
        }

        [Command("Mute")]
        [Alias("silence")]
        [Summary("Mute a user until they are manually unmuted.")]
        public async Task Mute([Summary("The user you want to mute.")] IGuildUser userToMute, [Summary("The reason for muting the user.")][Remainder] string reason = null)
        {
            var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

            if (mutedRole == null)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "there is no muted role set for this server. Please use the ``SetMutedRole`` command to remedy this error.");
                return;
            }
            else if (_moderationService.GetPermissionLevel(Context.DbGuild, userToMute) > 0)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"{userToMute.Mention} is a moderator and thus cannot be muted.");
                return;
            }

            await userToMute.AddRoleAsync(mutedRole);

            await _muteRepository.InsertMuteAsync(userToMute, TimeSpan.FromDays(365));

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully muted {userToMute.Mention}.");

            string message = $"**{Context.User.Mention}** has permanently muted you in **{Context.Guild.Name}**";

            if (reason.Length > 0)
            {
                message += $" for **{reason}**";
            }

            await _moderationService.InformUserAsync((SocketUser)userToMute, message + ".");

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Mute", Configuration.MuteColor, reason, Context.User as IGuildUser, userToMute);
        }

        [Command("Unmute")]
        [Alias("unsilence")]
        [Summary("Unmute a muted user.")]
        public async Task Unmute([Summary("The user you want to unmute.")] IGuildUser userToUnmute, [Remainder] string reason = null)
        {
            if (!(await _muteRepository.AnyAsync(x => x.UserId == userToUnmute.Id && x.GuildId == Context.Guild.Id)))
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "that user is not muted and thus cannot be unmuted.");
                return;
            }

            await _muteRepository.DeleteAsync(x => x.UserId == userToUnmute.Id && x.GuildId == Context.Guild.Id);

            await userToUnmute.RemoveRoleAsync(Context.Guild.GetRole(Context.DbGuild.MutedRoleId));

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully unmuted **{userToUnmute.Mention}**.");

            string message = $"**{Context.User.Mention}** has unmuted you in **{Context.Guild.Name}**";

            if (reason.Length > 0)
            {
                message += $" for **{reason}**";
            }
            await _moderationService.InformUserAsync((SocketUser)userToUnmute, message + ".");

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Unmute", Configuration.UnmuteColor, reason, Context.User as IGuildUser, userToUnmute);
        }

        //[Command("custommute")]
        //[Alias("customsilence")]
        //[Summary("Mute a user for a set amount of time.")]
        //public async Task CustomMuteAsync(double hours, IGuildUser userToMute, [Remainder] string reason = null)
        //{
        //    if (hours < 1)
        //    {
        //        await _text.ReplyErrorAsync(Context.User, Context.Channel, "you cannot mute anyone for less than 1 hour.");

        //        return;
        //    }

        //    var time = hours == 1 ? "hour" : "hours";

        //    var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

        //    if (mutedRole == null)
        //    {
        //        await _text.ReplyErrorAsync(Context.User, Context.Channel, "there is no muted role set for this server. Please use the ``SetMutedRole`` command to remedy this error.");

        //        return;
        //    }

        //    await userToMute.AddRoleAsync(mutedRole);

        //    await _muteRepository.InsertMuteAsync(userToMute, TimeSpan.FromHours(hours));

        //    await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully muted {userToMute.Mention} for {hours} {time}.");

        //    // use moderation service to try and DM the usertoMute
        //    // log it to modlog
        //}

        [Command("Kick")]
        [Alias("boot")]
        [Summary("Kick any player from the server.")]
        public async Task Kick([Summary("The user you wish to kick.")] IGuildUser userToKick, [Summary("The reason for kicking the user.")][Remainder] string reason = null)
        {
            if (_moderationService.GetPermissionLevel(Context.DbGuild, userToKick) > 0)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, $"{userToKick.Mention} is a moderator and thus cannot be kicked.");
                return;
            }

            string message = $"{Context.User.Mention} has kicked you from **{Context.Guild.Name}**";
            string reply = $"You have successfully kicked {userToKick.Mention}";

            if (reason.Length > 0)
            {
                message += $" for **{reason}**";
                reply += $" for **{reason}**";
            }

            await _moderationService.InformUserAsync((SocketUser)userToKick, message + ".");

            await userToKick.KickAsync(reason);

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Kick", Configuration.KickColor, reason, Context.User as IGuildUser, userToKick);

            await _text.ReplyAsync(Context.User, Context.Channel, reply + ".");
        }

        [Command("Clear")]
        [Alias("clearchat", "purge", "purgechat")]
        [Summary("Clear any amount of messages from the channel this command is ran in.")]
        public async Task Clear([Summary("The amount of messages to clear.")] int amountOfMessages = 50, [Summary("The reason for clearing the chat.")][Remainder] string reason = null)
        {
            if (amountOfMessages < 1 || amountOfMessages > 150)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "you must clear more than 1 and less than 150 messages.");
                return;
            }

            var channel = Context.Channel as ITextChannel;
            var messages = await channel.GetMessagesAsync(amountOfMessages + 1).Flatten().ToArray();

            await channel.DeleteMessagesAsync(messages);

            await _moderationService.ModLogAsync(Context.DbGuild, Context.Guild, "Clear", Configuration.ClearColor, reason, Context.User as IGuildUser, null);

            var message = await ReplyAsync($"You have successfully cleared **{amountOfMessages}** messages.");

            await Task.Delay(2000);

            await message.DeleteAsync();
        }
    }
}