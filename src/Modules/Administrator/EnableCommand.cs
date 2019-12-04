using Discord.Commands;
using NukoBot.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    public partial class Administrator
    {
        [Command("EnableCommand")]
        [Alias("enable", "allow", "allowcommand")]
        [Summary("Unban a specified command from being ran in this server.")]
        [Remarks("kick")]
        public async Task EnableCommand(string commandOrAlias)
        {
            var foundCommand = _commandService.Commands.Where(x => x.Aliases.Any(x => x.ToLower() == commandOrAlias.ToLower())).FirstOrDefault();

            if (foundCommand == null)
            {
                await ReplyErrorAsync("no command with that name was found.");
                return;
            }

            if (!(Context.DbGuild.DisabledCommands.Any(x => x.ToLower() == foundCommand.Name.ToLower())))
            {
                await ReplyErrorAsync("that command is not disabled.");
                return;
            }

            var disabledCommands = Context.DbGuild.DisabledCommands.ToList();

            foreach (var name in foundCommand.Aliases)
            {
                disabledCommands.Remove(name);
            }

            var disabledCommandsArray = disabledCommands.ToArray();

            await _guildRepository.ModifyAsync(Context.DbGuild, x => x.DisabledCommands = disabledCommandsArray);

            await ReplyAsync($"you have have successfully enabled the `{Configuration.Prefix}{foundCommand.Name}` command.");
        }
    }
}
