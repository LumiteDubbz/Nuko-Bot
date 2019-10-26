using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Services;
using System;
using System.Threading.Tasks;

namespace NukoBot.Common.Preconditions.Command
{
    public sealed class RequireAdministrator : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            var nukoContext = new Context(context.Message as SocketUserMessage, serviceProvider);

            await nukoContext.InitializeAsync();

            serviceProvider.GetRequiredService<ModerationService>();

            var moderationService = serviceProvider.GetRequiredService<ModerationService>();
            var permissionLevel = moderationService.GetPermissionLevel(nukoContext.DbGuild, context.User as IGuildUser);

            if (permissionLevel < 2) return await Task.FromResult(PreconditionResult.FromError("This command may only by used by users with a permission level of at least 2 or the Administrator role permission."));

            return await Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}