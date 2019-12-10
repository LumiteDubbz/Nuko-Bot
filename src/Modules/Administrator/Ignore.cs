using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("Ignore")]
        [Alias("acknowledge", "recognise", "recognize", "toggleignore")]
        [Summary("Specify a channel for the bot to ignore messages from.")]
        [Remarks("#general")]
        public async Task Ignore(IMessageChannel channel = null)
        {
            var message = "you have successfully ";
            var ignoredChannels = Context.DbGuild.IgnoredChannels.ToList();

            string action;

            channel ??= Context.Channel;

            if (Context.DbGuild.IgnoredChannels.Any(x => x == channel.Id)) {
                ignoredChannels.Remove(channel.Id);

                action = "**acknowledged**";
            }
            else
            {
                ignoredChannels.Add(channel.Id);

                action = "**ignored**";
            }

            var ignoredChannelsAsArray = ignoredChannels.ToArray();

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.IgnoredChannels = ignoredChannelsAsArray);

            message += $"{action} this channel.";

            await ReplyAsync(message);
        }
    }
}
