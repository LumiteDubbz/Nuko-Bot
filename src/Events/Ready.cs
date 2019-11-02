using Discord.WebSocket;
using System.Threading.Tasks;
using NukoBot.Common;
using System;

namespace NukoBot.Events
{
    public sealed class Ready
    {
        private readonly DiscordSocketClient _client;
        private readonly Credentials _credentials;

        public Ready(DiscordSocketClient client, Credentials credentials)
        {
            _client = client;
            _credentials = credentials;

            _client.Ready += HandleReadyAsync;
        }

        private async Task HandleReadyAsync()
        {
            await _client.SetGameAsync(Configuration.Game + $" | v{_credentials.Version}");

            Console.WriteLine($"{_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator} ({_client.CurrentUser.Id}) has connected to Discord.");
        }
    }
}