using Discord.Commands;
using NukoBot.Common.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("RemoveCustomCommand")]
        [Alias("deletecustomcommand", "removecustom", "deletecustom")]
        [Summary("Remove a custom command.")]
        [Remarks("google")]
        public async Task RemoveCustomCommand(string name)
        {
            if (Context.DbGuild.CustomCommands.Any(x => x.Name.ToLower() == name.ToLower()))
            {
                await _guildRepository.ModifyAsync(Context.DbGuild, x => x.CustomCommands.Remove(name.ToLower()));

                await ReplyAsync($"you have successfully removed the **{name.WithUppercaseFirstCharacter()}** custom command.");
            }
            else
            {
                await ReplyErrorAsync($"the custom command **{name.WithUppercaseFirstCharacter()}** does not exist.");
            }
        }
    }
}
