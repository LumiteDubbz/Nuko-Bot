using Discord.Commands;
using NukoBot.Common;
using System;

namespace NukoBot.Modules.System
{
    [Name("System")]
    [Summary("Commands relating to the core functions of the bot.")]
    public partial class System : Module
    {
        private readonly CommandService _commandService;

        public System(CommandService commandService, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _commandService = commandService;
        }
    }
}
