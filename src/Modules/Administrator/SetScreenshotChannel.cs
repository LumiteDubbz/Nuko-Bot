using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetScreenshotChannel")]
        [Alias("removescreenshotchannel")]
        [Summary("Set the channel for screenshots to be submitted to.")]
        public async Task SetScreenshotChannel([Summary("The channel you want to set as the screenshot submitting channel.")][Remainder] ITextChannel screenshotChannel = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ScreenshotChannelId = screenshotChannel.Id);

            if (screenshotChannel != null)
            {
                await ReplyAsync($"you have successfully set the screenshot channel to {screenshotChannel.Mention}.");
                return;
            }

            await ReplyAsync("you have successfully removed the screenshot channel.");
        }
    }
}
