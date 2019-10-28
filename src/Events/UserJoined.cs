using Discord.WebSocket;
using System.Threading.Tasks;
using NukoBot.Services;
using Discord;
using System;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Database.Repositories;

namespace NukoBot.Events
{
    public sealed class UserJoined
    {
        private readonly DiscordSocketClient _client;
        private readonly IServiceProvider _serviceProvider;
        private readonly GuildRepository _guildRepository;
        private readonly Text _text;

        public UserJoined(DiscordSocketClient client, IServiceProvider serviceProvider)
        {
            _client = client;
            _serviceProvider = serviceProvider;
            _guildRepository = _serviceProvider.GetRequiredService<GuildRepository>();
            _text = _serviceProvider.GetRequiredService<Text>();

            _client.UserJoined += HandleUserJoinedAsync;
        }

        private async Task HandleUserJoinedAsync(SocketGuildUser socketUser)
        {
            var user = socketUser as IGuildUser;

            var dbGuild = await _guildRepository.GetGuildAsync(user.GuildId);

            await user.AddRoleAsync(user.Guild.GetRole(dbGuild.NewUserRole));

            if (dbGuild.WelcomeMessage != null)
            {
                var userDm = await user.GetOrCreateDMChannelAsync();

                await _text.SendAsync(userDm, dbGuild.WelcomeMessage);
            }
        }
    }
}