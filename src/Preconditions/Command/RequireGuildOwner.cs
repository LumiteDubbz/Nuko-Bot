using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace NukoBot.Preconditions.Command
{
    public sealed class RequireGuildOwner : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            if (context.Guild.OwnerId != context.User.Id) return Task.FromResult(PreconditionResult.FromError("This command may only by used by the guild owner."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}