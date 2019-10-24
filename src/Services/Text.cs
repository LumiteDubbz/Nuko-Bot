using Discord;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Services
{
    public sealed class Text
    {
        public Task SendAsync(IMessageChannel channel, string description, string title = null, Color? color = null)
        {
            var builder = new EmbedBuilder
            {
                Color = color ?? Configuration.Color(),
                Description = description,
                Title = title
            };

            return channel.SendMessageAsync("", false, builder.Build());
        }

        public Task ReplyAsync(IUser user, IMessageChannel channel, string description, string title = null, Color? color = null)
        {
            return SendAsync(channel, user.Mention + ", " + description, title, color);
        }

        public Task ReplyErrorAsync(IUser user, IMessageChannel channel, string description)
        {
            return ReplyAsync(user, channel, description, null, Configuration.ErrorColor);
        }
    }
}