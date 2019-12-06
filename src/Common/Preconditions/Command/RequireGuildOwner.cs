using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace NukoBot.Common.Preconditions.Command
{
    public sealed class RequireGuildOwner : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            if (context.Channel is IDMChannel) return await Task.FromResult(PreconditionResult.FromError("this command can only be in a guild."));

            if (context.Guild.OwnerId != context.User.Id) return await Task.FromResult(PreconditionResult.FromError("this command may only by used by the guild owner."));

            return await Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}