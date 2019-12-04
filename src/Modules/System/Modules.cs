using Discord.Commands;
using NukoBot.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.System
{
    public partial class System
    {
        [Command("Commands")]
        [Alias("modules", "module", "command", "manual")]
        [Summary("View all modules or all commands in a specific module.")]
        [Remarks("administrator")]
        public async Task Commands(string commandOrModule = null)
        {
            var userDm = await Context.User.GetOrCreateDMChannelAsync();

            if (commandOrModule != null)
            {
                commandOrModule = commandOrModule.ToLower();

                var foundCommand = _commandService.Commands.SingleOrDefault(x => x.Name.ToLower() == commandOrModule);
                var foundModule = _commandService.Modules.SingleOrDefault(x => x.Name.ToLower() == commandOrModule);
                var commandsInModule = string.Empty;
                var commandParameters = string.Empty;
                var message = $"`{commandOrModule}` could refer to:\n\n";

                if (foundCommand != null)
                {
                    if (foundCommand.Parameters.Any())
                    {
                        foreach (var parameter in foundCommand.Parameters)
                        {
                            if (parameter.IsOptional)
                            {
                                commandParameters += $"[{parameter.Name}] ";
                            }
                            else
                            {
                                commandParameters += $"<{parameter.Name}> ";
                            }
                        }
                    }

                    message += $"`{Configuration.Prefix}{foundCommand.Name}";

                    if (commandParameters.Any())
                    {
                        message += $" {commandParameters.Remove(commandParameters.Length - 1)}";
                    }

                    message += $"`: {foundCommand.Summary}";
                }

                if (foundModule != null)
                {
                    foreach (var command in foundModule.Commands)
                    {
                        commandParameters = string.Empty;

                        if (command.Parameters.Any())
                        {
                            foreach (var parameter in command.Parameters)
                            {
                                if (parameter.IsOptional)
                                {
                                    commandParameters += $"[{parameter.Name}] ";
                                }
                                else
                                {
                                    commandParameters += $"<{parameter.Name}> ";
                                }
                            }
                        }

                        commandsInModule += $"`{Configuration.Prefix}{command.Name}";

                        if (commandParameters.Any())
                        {
                            commandsInModule += $" {commandParameters.Remove(commandParameters.Length - 1)}";
                        }

                        commandsInModule += $"`: {command.Summary}\n\n";
                    }

                    message += $"__Commands in the {foundModule.Name} module__:\nParameters in [square brackets] are optional, those in <angle brackets> are required.\n\nIf optional parameters are left blank, they will refer to their default values. This usually just resets to `null` or nothing.\n\n{commandsInModule}\n\n";
                }

                await DmAsync(Context.User, message, "Command information");

                if (Context.Channel != userDm) await ReplyAsync("please check your DMs.");
                return;
            }

            var modules = string.Empty;

            foreach (var module in _commandService.Modules)
            {
                modules += $"**{module.Name}**: {module.Summary}\n\n";
            }

            await DmAsync(Context.User, $"{modules.Remove(modules.Length - 2)}\n\nTo view the commands in any given module, please use the `{Configuration.Prefix}module [moduleName]`.", "Module information");

            if (Context.Channel != userDm) await ReplyAsync("please check your DMs.");
        }
    }
}
