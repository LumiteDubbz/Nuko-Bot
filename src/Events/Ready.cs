using Discord.WebSocket;
using System.Threading.Tasks;
using NukoBot.Common;
using NukoBot.Services;
using Discord;

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
            await _client.SetGameAsync(Configuration.Game);

            await new Logger().LogAsync(LogSeverity.Info, $"{_client.CurrentUser.Username}#{_client.CurrentUser.Discriminator} ({_client.CurrentUser.Id}) has connected to Discord.");
        }
    }
}