using Discord.Commands;
using NukoBot.Common.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("AddCustomCommand")]
        [Alias("createcustomcommand", "makecustomcommand", "customcommand", "createcustom", "createcommand", "makecustom", "makecommand", "custom")]
        [Summary("Create a command that sends the given response.")]
        [Remarks("google https://google.co.uk")]
        public async Task AddCustomCommand(string name, [Remainder] string response)
        {
            if (_commandService.Commands.Any(x => x.Name.ToLower() == name.ToLower() || x.Aliases.Any(x => x.ToLower() == name.ToLower())) || Context.DbGuild.CustomCommands.Any(x => x.Name.ToLower() == name.ToLower())) {
                await ReplyErrorAsync("that command already exists.");
                return;
            }

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.CustomCommands.Add(name.ToLower(), response));

            await ReplyAsync($"you have successfully added **{name.WithUppercaseFirstCharacter()}** as a custom command with **{response}** as its response.");
        }
    }
}
