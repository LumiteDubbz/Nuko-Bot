using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Preconditions.Command;
using NukoBot.Database.Repositories;
using NukoBot.Services;
using System;
using System.Threading.Tasks;

namespace NukoBot.Modules
{
    [Name("BotOwner")]
    [Summary("Commands only allowed to be used by the owners of this bot.")]
    [RequireBotOwner]
    public sealed class BotOwner : ModuleBase<Context>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;

        public BotOwner(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
        }

        [Command("LeaveGuild")]
        [Alias("leaveserver")]
        [Summary("Leaves any guild with a supplied ID.")]
        public async Task LeaveGuild([Summary("The ID of the guild you want the bot to leave")] ulong guildId)
        {
            var guild = await (Context.Client as IDiscordClient).GetGuildAsync(guildId);

            if (guild == null)
            {
                await _text.ReplyErrorAsync(Context.User, Context.Channel, "That guild was not found, meaning the ID was wrong or this bot is not in that guild.");
                return;
            }

            await guild.LeaveAsync();

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully removed this bot from the server **{guild.Name}**");
        }

        [Command("SetGame")]
        [Alias("setstatus")]
        [Summary("Sets the current playing status for the bot")]
        public async Task SetGame([Summary("The status you want to set.")] [Remainder] string game = null)
        {
            if (game == null)
            {
                game = Configuration.Game;
            }

            await Context.Client.SetGameAsync(game);

            await _text.ReplyAsync(Context.User, Context.Channel, $"you have successfully set the game to **{game}**.");
        }
    }
}
