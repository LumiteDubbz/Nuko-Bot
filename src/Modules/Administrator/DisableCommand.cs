using Discord.Commands;
using NukoBot.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("DisableCommand")]
        [Alias("bancommand", "restrictcommand", "disable")]
        [Summary("Ban a specificed command from being ran in this server.")]
        [Remarks("kick")]
        public async Task DisableCommand(string commandOrAlias)
        {
            var foundCommand = _commandService.Commands.Where(x => x.Aliases.Any(x => x.ToLower() == commandOrAlias.ToLower())).FirstOrDefault();

            if (foundCommand == null)
            {
                await ReplyErrorAsync("no command with that name was found.");
                return;
            }

            var listOfDisabledCommands = Context.DbGuild.DisabledCommands.ToList();
            
            if (listOfDisabledCommands.Any(x => x == foundCommand.Name))
            {            
                await ReplyErrorAsync("that command has already been disabled.");
                return;
            }

            foreach (var name in foundCommand.Aliases)
            {
                listOfDisabledCommands.Add(name);
            }

            var arrayOfDisabledCommands = listOfDisabledCommands.ToArray();

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.DisabledCommands = arrayOfDisabledCommands);

            await ReplyAsync($"you have have successfully disabled the `{Configuration.Prefix}{foundCommand.Name}` command.");
        }
    }
}
