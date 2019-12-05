using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Database.Repositories;
using NukoBot.Services;
using System;

namespace NukoBot.Modules.General
{
    [Name("General")]
    [Summary("General commands not directly related to the core functionality of the bot.")]
    [RequireContext(ContextType.Guild)]
    public partial class General : Module
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserRepository _userRepository;
        private readonly PollRepository _pollRepository;
        private readonly PointService _pointService;

        public General(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
            _pollRepository = _serviceProvider.GetRequiredService<PollRepository>();
            _pointService = _serviceProvider.GetRequiredService<PointService>();
        }
    }
}