using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Services;
using System;
using System.Threading.Tasks;

namespace NukoBot.Common
{
    public abstract class Module : ModuleBase<Context>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;

        public Module(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
        }

        public async Task ReplyAsync(string message)
        {
            await _text.ReplyAsync(Context.User, Context.Channel, message);
        }

        public async Task ReplyErrorAsync(string message)
        {
            await _text.ReplyErrorAsync(Context.User, Context.Channel, message);
        }

        public async Task SendAsync(string message, string title = null, Color? color = null, string imageUrl = null)
        {
            await _text.SendAsync(Context.Channel, message, title, color, imageUrl);
        }

        public async Task DmAsync(IUser user, string message, string title = null, Color? color = null, string imageUrl = null)
        {
            var userDm = await user.GetOrCreateDMChannelAsync();

            await _text.SendAsync(userDm, message, title, color, imageUrl);
        }

        public async Task SendScreenshotAsync(IMessageChannel screenshotChannel, string message, Attachment image)
        {
            await _text.SendScreenshotAsync(screenshotChannel, message, image);
        }

        public async Task SendImageAsync(string message, string imageUrl)
        {
            await _text.SendImageAsync(Context.Channel, message, imageUrl);
        }
    }
}
