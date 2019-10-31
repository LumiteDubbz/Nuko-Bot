using Discord;
using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Common.Preconditions.Command
{
    public sealed class RequireAttachedImage : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider serviceProvider)
        {
            var attachment = context.Message.Attachments.FirstOrDefault();

            if (attachment == null) return Task.FromResult(PreconditionResult.FromError("You must attach an image."));

            return Task.FromResult(PreconditionResult.FromSuccess());
        }
    }
}