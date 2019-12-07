using Discord;
using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Modules.BotOwner
{
    public partial class BotOwner
    {
        [Command("LeaveGuild")]
        [Alias("leaveserver")]
        [Summary("Leaves any guild with a supplied ID.")]
        [Remarks("636682997801943051")]
        public async Task LeaveGuild([Summary("The ID of the guild you want the bot to leave")] ulong guildId)
        {
            var guild = await (Context.Client as IDiscordClient).GetGuildAsync(guildId);

            if (guild == null)
            {
                await ReplyErrorAsync("that guild was not found, meaning the ID was wrong or this bot is not in that guild.");
                return;
            }

            await guild.LeaveAsync();

            await ReplyAsync($"you have successfully removed this bot from the server **{guild.Name}**");
        }
    }
}
