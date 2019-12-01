using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Preconditions.Command;
using NukoBot.Database.Repositories;
using System;

namespace NukoBot.Modules.Owner
{
    [Name("Owner")]
    [Summary("These commands can only be used by the owner of the server they are ran in.")]
    [RequireContext(ContextType.Guild)]
    [RequireGuildOwner]
    public partial class Owner : Module
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GuildRepository _guildRepository;

        public Owner(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _guildRepository = _serviceProvider.GetRequiredService<GuildRepository>();
        }
    }
}