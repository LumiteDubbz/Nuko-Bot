using Discord.Commands;
using NukoBot.Common;
using System.Threading.Tasks;

namespace NukoBot.Services.Handlers
{
    public sealed class ErrorHandler
    {
        public Task<string> HandleCommandErrorAsync(IResult result, string commandUsed)
        {
            switch (result.Error)
            {
                case CommandError.BadArgCount:
                    return Task.FromResult($"you did not provide the appropriate arguments for that command. Please use `{Configuration.Prefix}command {commandUsed}`.");
                case CommandError.Exception: case CommandError.Unsuccessful:
                    return Task.FromResult($"an error occurred in the backend code of the command execution. Please report the following error [here]({Configuration.GitRepository}/issues):\n```{result.Error}```");
                default:
                    return null;
            }
        }
    }
}
