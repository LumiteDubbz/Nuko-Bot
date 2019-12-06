using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Services;
using System;
using System.Threading.Tasks;

namespace NukoBot.Common.Preconditions.Command
{
    public sealed class RequireModerator : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            if (context.Channel is IDMChannel) return await Task.FromResult(PreconditionResult.FromError("this command can only be in a guild."));

            var nukoContext = new Context(context.Message as SocketUserMessage, serviceProvider);

            await nukoContext.InitializeAsync();

            serviceProvider.GetRequiredService<ModerationService>();

            var moderationService = serviceProvider.GetRequiredService<ModerationService>();
            var permissionLevel = moderationService.GetPermissionLevel(nukoContext.DbGuild, context.User as IGuildUser);

            if (permissionLevel < 1) return await Task.FromResult(PreconditionResult.FromError("this command may only by used by users with a permission level of at least 1."));

            return await Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}