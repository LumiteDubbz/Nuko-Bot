using Discord.WebSocket;
using System.Threading.Tasks;
using NukoBot.Common;
using NukoBot.Services;
using Discord;
using Discord.Commands;
using System;

namespace NukoBot.Events
{
    public sealed class MessageReceived
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        // private readonly Text _text;

        public MessageReceived(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider)
        {
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            // _text = _serviceProvider.GetRequiredService<Text>();

            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            Console.WriteLine(socketMessage.Content);
        }
    }
}
