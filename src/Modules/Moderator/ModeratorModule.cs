using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Preconditions.Command;
using NukoBot.Database.Repositories;
using NukoBot.Services;
using System;

namespace NukoBot.Modules.Moderator
{
    [Name("Moderator")]
    [Summary("Commands only allowed to be used by users with a role with a permission level of at least 1.")]
    [RequireModerator]
    public partial class Moderator : Module
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserRepository _userRepository;
        private readonly PollRepository _pollRepository;
        private readonly MuteRepository _muteRepository;
        private readonly ModerationService _moderationService;

        public Moderator(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
            _pollRepository = _serviceProvider.GetRequiredService<PollRepository>();
            _muteRepository = _serviceProvider.GetRequiredService<MuteRepository>();
            _moderationService = _serviceProvider.GetRequiredService<ModerationService>();
        }
    }
}