using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Preconditions.Command;
using NukoBot.Services;
using System;

namespace NukoBot.Modules.BotOwner
{
    [Name("BotOwner")]
    [Summary("Commands only allowed to be used by the owners of this bot.")]
    [RequireBotOwner]
    public partial class BotOwner : Module
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly EvaluationService _evaluationService;

        public BotOwner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _evaluationService = _serviceProvider.GetRequiredService<EvaluationService>();
        }
    }
}