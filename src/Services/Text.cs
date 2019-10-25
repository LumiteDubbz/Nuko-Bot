using Discord;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Services
{
    public sealed class Text
    {
        public Task SendAsync(IMessageChannel channel, string description, string title = null, Color? color = null, string imageUrl = null)
        {
            var builder = new EmbedBuilder
            {
                Color = color ?? Configuration.Color(),
                Description = description,
                Title = title,
                ImageUrl = imageUrl
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

        public Task SendScreenshotAsync(IMessageChannel channel, string description, Attachment image)
        {
            return SendAsync(channel, description, null, null, image.Url);
        }
    }
}