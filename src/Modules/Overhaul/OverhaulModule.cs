using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Database.Repositories;
using NukoBot.Services;
using System;

namespace NukoBot.Modules.Overhaul
{
    [Name("Overhaul")]
    [Summary("Commands relating to the BTDB Overhaul mod.")]
    [RequireContext(ContextType.Guild)]
    public partial class Overhaul : Module
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserRepository _userRepository;
        private readonly PointService _pointService;

        public Overhaul(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
            _pointService = _serviceProvider.GetRequiredService<PointService>();
        }
    }
}