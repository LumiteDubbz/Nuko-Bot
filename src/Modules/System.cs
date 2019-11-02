using Discord.Commands;
using NukoBot.Common;
using NukoBot.Services;
using System.Threading.Tasks;
using System.Linq;
using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace NukoBot.Modules
{
    [Name("System")]
    [Summary("Commands relating to the core functions of the bot.")]
    public sealed class System : ModuleBase
    {
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;

        public System(CommandService commandService, IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
        }

        [Command("Help")]
        [Alias("info")]
        [Summary("View the basic info regarding this bot.")]
        public async Task Help()
        {
            var userDm = await Context.User.GetOrCreateDMChannelAsync();

            await _text.SendAsync(userDm, Configuration.HelpMessage);

            if (Context.Channel != userDm)
            {
                await _text.ReplyAsync(Context.User, Context.Channel, "please check your DMs.");
            }
        }

        [Command("Commands")]
        [Alias("modules", "module", "command")]
        [Summary("View all modules or all commands in a specific module.")]
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

                    message += $"__Commands in the {foundModule.Name} module__:\nParameters in [square brackets] are optional, those in <angle brackets> are required.\n\n{commandsInModule}\n\n";
                }

                await _text.SendAsync(userDm, message, "Command information");
                
                if (Context.Channel != userDm) await _text.ReplyAsync(Context.User, Context.Channel, "please check your DMs.");
                return;
            }

            var modules = string.Empty;

            foreach (var module in _commandService.Modules)
            {
                modules += $"**{module.Name}**: {module.Summary}\n\n";
            }

            await _text.SendAsync(userDm, $"{modules.Remove(modules.Length - 2)}\n\nTo view the commands in any given module, please use the `{Configuration.Prefix}module [moduleName]`.", "Module information");

            if (Context.Channel != userDm) await _text.ReplyAsync(Context.User, Context.Channel, "please check your DMs.");
        }

        [Command("Support")]
        [Alias("report-bug", "reportbug")]
        [Summary("Displays the invitation link to the support server.")]
        public async Task Support()
        {
            await _text.ReplyAsync(Context.User, Context.Channel, "for bot support, selfhosting support or feature requests, join the support server [here](" + Configuration.SupportServerLink + ").");
        }

        [Command("Credits")]
        [Alias("creds")]
        [Summary("View the owners of this bot.")]
        public async Task Credits()
        {
            List<string> authors = new List<string>();

            foreach (var author in Configuration.Authors)
            {
                authors.Add(author);
            }

            string message = "A huge and special thanks to the people who helped make this bot:\n";
            string separator = ", ";

            foreach (var author in authors)
            {
                message += $"**{author}**" + separator;
            }

            await _text.SendAsync(Context.Channel, message.Remove(message.Length - separator.Length) + ".");
        }
    }
}