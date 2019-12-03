using Discord.WebSocket;
using System.Threading.Tasks;
using NukoBot.Common;
using NukoBot.Services;
using Discord;
using Discord.Commands;
using System;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Services.Handlers;

namespace NukoBot.Events
{
    public sealed class MessageReceived
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ErrorHandler _errorHandler;
        private readonly Text _text;

        public MessageReceived(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider)
        {
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _errorHandler = _serviceProvider.GetRequiredService<ErrorHandler>();
            _text = _serviceProvider.GetRequiredService<Text>();

            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;

            if (message.Source != MessageSource.User) return;

            var argPos = 0;

            if (!message.HasStringPrefix(Configuration.Prefix, ref argPos)) return;

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
                
                var commandUsed = message.Content.Remove(0, 1).Split(" ")[0];
                var response = await _errorHandler.HandleCommandErrorAsync(result, commandUsed);

                await _text.ReplyErrorAsync(context.User, context.Channel, response ?? "an unknown error occurred.");
            }
        }
    }
}