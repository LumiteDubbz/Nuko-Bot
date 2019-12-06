using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Common.Preconditions.Command
{
    public sealed class RequireAttachedImage : PreconditionAttribute
    {
        public async override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            if (context.Channel is IDMChannel) return await Task.FromResult(PreconditionResult.FromError("this command can only be in a guild."));

            var attachment = context.Message.Attachments.FirstOrDefault();

            if (attachment == default(Attachment)) return await Task.FromResult(PreconditionResult.FromError("you must attach an image."));

            return await Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}