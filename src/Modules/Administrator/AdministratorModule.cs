using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Preconditions.Command;
using NukoBot.Common.Structures;
using NukoBot.Database.Repositories;
using NukoBot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.Administrator
{
    [Name("Administrator")]
    [Summary("Commands only allowed to be used by users with a role with a permission level of at least 2.")]
    [RequireAdministrator]
    public partial class Administrator : Module
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GuildRepository _guildRepository;
        private readonly UserRepository _userRepository;
        private readonly ModerationService _moderationService;
        private readonly PointService _pointService;

        public Administrator(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _guildRepository = _serviceProvider.GetRequiredService<GuildRepository>();
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
            _moderationService = _serviceProvider.GetRequiredService<ModerationService>();
            _pointService = _serviceProvider.GetRequiredService<PointService>();
        }
    }
}