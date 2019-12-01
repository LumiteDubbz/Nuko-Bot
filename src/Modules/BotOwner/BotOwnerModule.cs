using Discord.Commands;
using NukoBot.Common;
using NukoBot.Common.Preconditions.Command;
using System;

namespace NukoBot.Modules.BotOwner
{
    [Name("BotOwner")]
    [Summary("Commands only allowed to be used by the owners of this bot.")]
    [RequireBotOwner]
    public partial class BotOwner : Module
    {
        public BotOwner(IServiceProvider serviceProvider) : base(serviceProvider) { }
    }
}