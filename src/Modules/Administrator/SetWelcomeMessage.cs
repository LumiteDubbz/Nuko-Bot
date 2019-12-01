using Discord.Commands;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetWelcomeMessage")]
        [Alias("setwelcomemsg", "deletewelcomemessage", "deletewelcomemsg", "removewelcomemessage", "removewelcomemsg")]
        [Summary("Set the message to sent when a user joins this server.")]
        public async Task SetWelcomeChannel([Summary("The welcome message to be DMed to new users.")][Remainder] string welcomeMessage = null)
        {
            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.WelcomeMessage = welcomeMessage);

            if (welcomeMessage != null)
            {
                await ReplyAsync($"you have successfully set the welcome message to **{welcomeMessage}**");
                return;
            }

            await ReplyAsync("you have successfully removed the welcome message.");
        }
    }
}
