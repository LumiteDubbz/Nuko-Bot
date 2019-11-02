using Discord.WebSocket;
using System.Threading.Tasks;
using NukoBot.Common;
using System;

namespace NukoBot.Events
{
    public sealed class Ready
    {
        private readonly DiscordSocketClient _client;

        public Ready(DiscordSocketClient client)
        {
            _client = client;

            _client.Ready += HandleReadyAsync;
        }

        private async Task HandleReadyAsync()
        {
            await _client.SetGameAsync(Configuration.Game + $" | v{Configuration.Version}");

            Console.WriteLine($"{_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator} ({_client.CurrentUser.Id}) v{Configuration.Version} has connected to Discord.");
        }
    }
}