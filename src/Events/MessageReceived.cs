using Discord.WebSocket;
using System.Threading.Tasks;
using NukoBot.Common;
using NukoBot.Services;
using Discord;
using Discord.Commands;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace NukoBot.Events
{
    public sealed class MessageReceived
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;
        private readonly AutomaticHelpService _automaticHelpService;

        public MessageReceived(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider)
        {
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
            _automaticHelpService = _serviceProvider.GetRequiredService<AutomaticHelpService>();

            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;

            if (message.Source != MessageSource.User) return;

            var argPos = 0;

            if (!message.HasStringPrefix(Configuration.Prefix, ref argPos))
            {
                var checkForTriggerWord = _automaticHelpService.HasTriggerWord(message);

                if (checkForTriggerWord != null)
                {
                    await _automaticHelpService.HandleHelpRequestAsync(new Context(message, _serviceProvider, _client), checkForTriggerWord);
                    return;
                }

                return;
            }

            var context = new Context(message, _serviceProvider, _client);

            if (!(message.Channel is IDMChannel))
            {
                await context.InitializeAsync();
            }

            var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);

            if (!result.IsSuccess)
            {
                if (result.Error == CommandError.UnknownCommand)
                {
                    return;
                }

                await _text.ReplyErrorAsync(message.Author, context.Channel, $"I'm sorry but an error occurred whilst executing that command:\n\n```{result.ErrorReason}```");
            }
        }
    }
}