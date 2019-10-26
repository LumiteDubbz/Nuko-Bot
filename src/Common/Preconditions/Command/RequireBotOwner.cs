using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Common.Preconditions.Command
{
    public sealed class RequireBotOwner : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            var credentials = serviceProvider.GetRequiredService<Credentials>();

            if (credentials.OwnerIds.Contains(context.User.Id)) return Task.FromResult(PreconditionResult.FromSuccess());

            return Task.FromResult(PreconditionResult.FromError("This command may only be used by thw owners of this bot."));
        }
    }
}