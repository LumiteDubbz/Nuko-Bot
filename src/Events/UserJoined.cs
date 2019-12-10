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

            if (dbGuild.NewUserRole != 0)
            {
                var role = user.Guild.GetRole(dbGuild.NewUserRole);

                if (role != null)
                {
                    await user.AddRoleAsync(user.Guild.GetRole(dbGuild.NewUserRole));
                }
            }

            var userDm = await user.GetOrCreateDMChannelAsync();

            if (dbGuild.LockedDown == true)
            {
                await _text.SendAsync(userDm, $"The server you just joined is currently on lockdown. This means that you and all other new users will be banned every time you join until the server is unlocked.");

                await user.BanAsync(0, "Server on lockdown.");
                return;
            }

            if (dbGuild.WelcomeMessage != null || dbGuild.WelcomeMessage.Length > 0)
            {
                await _text.SendAsync(userDm, dbGuild.WelcomeMessage);
            }
        }
    }
}