using Discord;
using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetModLogChannel")]
        [Alias("setlogchannel", "setmodlog", "removemodlogchannel", "removelogchannel")]
        [Summary("Set the channel for all moderator actions to be logged to.")]
        [Remarks("#mod-log")]
        public async Task SetModLogChannel([Summary("The channel you wish to set as the mod log channel.")][Remainder] ITextChannel modLogChannel = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.ModLogChannelId = modLogChannel.Id);

            if (modLogChannel != null)
            {
                await ReplyAsync($"you have successfully set the mod log channel to {modLogChannel.Mention}.");
                return;
            }

            await ReplyAsync("you have successfully removed the mod log channel.");
        }
    }
}
