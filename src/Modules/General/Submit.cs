using Discord;
using Discord.Commands;
using NukoBot.Common.Preconditions.Command;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.General
{
    public partial class General
    {
        [Command("Submit")]
        [Alias("submit-screenshot")]
        [Summary("Submit a screenshot of your game to the staff for points.")]
        [RequireAttachedImage]
        public async Task Submit()
        {
            if (Context.Message.Attachments.ElementAt(0) == null)
            {
                await ReplyErrorAsync("an unknown error occurred while submitting that image.");
                return;
            }

            await SendScreenshotAsync(Context.Client.GetChannel(Context.DbGuild.ScreenshotChannelId) as IMessageChannel, $"**{Context.Message.Author.Username}#{Context.Message.Author.Discriminator}** has submitted this screenshot.", Context.Message.Attachments.ElementAt(0));

            var message = await Context.Channel.SendMessageAsync("You have successfully submitted your screenshot for review.");

            await Task.Delay(2000);

            await Context.Message.DeleteAsync();

            await message.DeleteAsync();
        }
    }
}
