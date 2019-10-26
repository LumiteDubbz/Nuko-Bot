﻿using Discord.WebSocket;
using System.Threading.Tasks;
using NukoBot.Common;
using NukoBot.Services;
using Discord;
using Discord.Commands;
using System;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Database.Repositories;

namespace NukoBot.Events
{
    public sealed class MessageReceived
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commandService;
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;
        private readonly GuildRepository _guildRepo;

        public MessageReceived(DiscordSocketClient client, CommandService commandService, IServiceProvider serviceProvider)
        {
            _client = client;
            _commandService = commandService;
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
            _guildRepo = _serviceProvider.GetRequiredService<GuildRepository>();

            _client.MessageReceived += HandleMessageAsync;
        }

        private async Task HandleMessageAsync(SocketMessage socketMessage)
        {
            if (!(socketMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;

            if (!message.HasStringPrefix(Configuration.Prefix, ref argPos)) return;

            Console.WriteLine("message.HasStringPrefix() passed");

            var context = new Context(message, _serviceProvider, _client);

            Console.WriteLine("new Context() passed");

            await context.InitializeAsync();

            Console.WriteLine("InitializeAsync() passed");

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
