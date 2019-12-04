using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Services.Handlers
{
    public sealed class ErrorHandler
    {
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;

        public ErrorHandler(CommandService commandService, IServiceProvider serviceProvider)
        {
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
        }

        public async Task HandleCommandErrorAsync(IResult result, Context context)
        {
            var args = context.Message.Content.Split(' ');
            var commandName = args.First().StartsWith(Configuration.Prefix) ? args.First().Remove(0, Configuration.Prefix.Length) : args[1];
            var commandInfo = _commandService.Commands.Where(x => x.Aliases.Any(x => x.ToLower() == commandName.ToLower())).First();
            string messageResponse;
            string commandExample;

            if (result.Error == CommandError.UnknownCommand) return;

            switch (result.Error)
            {
                //case CommandError.UnknownCommand:
                //    commandName = commandInfo.Name;
                // bring back string extensions, add IsSimilarTo() method that operates on this string (so if (thing.IsSimilarTo(commandName) do stuff
                //    break;
                case CommandError.BadArgCount:
                    commandName = commandInfo.Name;
                    commandExample = commandInfo.Parameters.Count == 0 ? string.Empty : $"**Example of this command:**\n`{Configuration.Prefix}{commandName} {commandInfo.Remarks}`";
                    messageResponse = $"you did not provide the appropriate arguments for that command.\n\n**Command structure:**\n`{Configuration.Prefix}{commandName}{commandInfo.GetUsage()}`\n\n" + commandExample;
                    break;
                case CommandError.Exception:
                    if (result is ExecuteResult executeResult)
                    {
                        messageResponse = $"an error occurred in the backend code of the command execution. Please report the following error [here]({Configuration.GitRepository}/issues):\n```{executeResult.Exception.StackTrace}```";
                    }

                    messageResponse = $"an error occurred in the backend code of the command execution. Please report the following error [here]({Configuration.GitRepository}/issues):\n```{result.Error}```";
                    break;
                case CommandError.Unsuccessful:
                    messageResponse = $"an error occurred in the backend code of the command execution. Please report the following error [here]({Configuration.GitRepository}/issues):\n```{result.ErrorReason}```";
                    break;
                case CommandError.ParseFailed:
                    commandName = commandInfo.Name;
                    commandExample = commandInfo.Parameters.Count == 0 ? string.Empty : $"**Example of this command:**\n`{Configuration.Prefix}{commandName} {commandInfo.Remarks}`";
                    messageResponse = $"you provided the wrong type of argument for that command.\n\n**Command structure:**\n`{Configuration.Prefix}{commandName}{commandInfo.GetUsage()}`\n\n" + commandExample;
                    break;
                default:
                    messageResponse = result.ErrorReason;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(messageResponse))
            {
                await _text.ReplyErrorAsync(context.User, context.Channel, messageResponse);
            }
            else
            {
                return;
            }
        }
    }
}
