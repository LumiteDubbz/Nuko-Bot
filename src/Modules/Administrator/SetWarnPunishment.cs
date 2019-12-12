using Discord.Commands;
using NukoBot.Common.Extensions;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("SetWarnPunishment")]
        [Alias("setwarnaction", "setwarningaction", "setwarningpunishment", "warnpunishment")]
        [Summary("Set the punishment to be given to users who exceed the maximum amount of warnings.")]
        [Remarks("mute")]
        public async Task SetWarnPunishment(string punishment)
        {
            switch(punishment.ToLower())
            {
                case "mute":
                    await _guildRepository.ModifyAsync(Context.DbGuild, x => x.WarnPunishment = "mute");
                    break;
                case "kick":
                    await _guildRepository.ModifyAsync(Context.DbGuild, x => x.WarnPunishment = "kick");
                    break;
                case "ban":
                    await _guildRepository.ModifyAsync(Context.DbGuild, x => x.WarnPunishment = "ban");
                    break;
                default:
                    await ReplyErrorAsync($"**{punishment.WithUppercaseFirstCharacter()}** is not a valid punishment. Available options are \"**mute**\", \"**kick**\" or \"**ban**\".");
                    return;
            }

            await ReplyAsync($"you have successfully set this server's warning punishment to **{punishment.WithUppercaseFirstCharacter()}**.");
        }
    }
}
