using Discord;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Services
{
    public sealed class Text
    {
        public Task SendAsync(IMessageChannel channel, string message, string title = null, Color? color = null, string imageUrl = null)
        {
            var builder = new EmbedBuilder
            {
                Color = color ?? Configuration.Color(),
                Description = message,
                Title = title,
                ImageUrl = imageUrl
            };

            return channel.SendMessageAsync("", false, builder.Build());
        }

        public Task ReplyAsync(IUser user, IMessageChannel channel, string message, string title = null, Color? color = null)
        {
            return SendAsync(channel, user.Mention + ", " + message, title, color);
        }

        public Task ReplyErrorAsync(IUser user, IMessageChannel channel, string message)
        {
            return ReplyAsync(user, channel, message, null, Configuration.ErrorColor);
        }

        public Task SendScreenshotAsync(IMessageChannel channel, string message, Attachment image)
        {
            return SendAsync(channel, message, null, null, image.Url);
        }

        public Task SendImageAsync(IMessageChannel channel, string message, string imageUrl)
        {
            return SendAsync(channel, message, null, null, imageUrl);
        }
    }
}