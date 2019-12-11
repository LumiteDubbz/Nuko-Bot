using Discord.WebSocket;
using System.Threading.Tasks;
using NukoBot.Common;
using Discord;
using Discord.Commands;
using System;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Services.Handlers;
using System.Linq;
using NukoBot.Common.Extensions;

namespace NukoBot.Events
{
    public sealed class MessageReceived
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly ErrorHandler _errorHandler;

        public MessageReceived(CommandService commandService, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _commandService = commandService;
            _errorHandler = _serviceProvider.GetRequiredService<ErrorHandler>();

            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;

            if (message.Source != MessageSource.User) return;

            var argPos = 0;

            var context = new Context(message, _serviceProvider, _client);

            if (!(message.Channel is IDMChannel))
            {
                await context.InitializeAsync();

                if (!message.HasStringPrefix(context.DbGuild.Prefix, ref argPos)) return;

                if (context.DbGuild.IgnoredChannels.Any(x => x == context.Channel.Id)) return;

                var args = context.Message.Content.Split(' ');
                var commandName = args.First().StartsWith(context.DbGuild.Prefix) ? args.First().Remove(0, context.DbGuild.Prefix.Length) : args[1];

                if (context.DbGuild.DisabledCommands.Any(x => x == commandName.WithLowercaseFirstCharacter()))
                {
                    return;
                }
            }

            var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);

            if (!result.IsSuccess) await _errorHandler.HandleCommandErrorAsync(result, context);
        }
    }
}