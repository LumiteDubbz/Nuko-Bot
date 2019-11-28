using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Database.Models;
using NukoBot.Database.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NukoBot.Services.Timers
{
    internal sealed class AutoUnmute
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly GuildRepository _guildRepository;
        private readonly MuteRepository _muteRepository;
        private readonly ModerationService _moderationService;
        private readonly Timer _timer;

        public AutoUnmute(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _guildRepository = _serviceProvider.GetRequiredService<GuildRepository>();
            _muteRepository = _serviceProvider.GetRequiredService<MuteRepository>();
            _moderationService = _serviceProvider.GetRequiredService<ModerationService>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(Unmute);

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.Zero, Configuration.AutoUnmuteCooldown);

            StateObj.TimerReference = _timer;
        }

        private void Unmute(object stateObj)
        {
            Task.Run(async () =>
            {
                List<Mute> collection = null;

                try
                {
                    collection = await _muteRepository.AllAsync();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }

                foreach (var mute in collection)
                {
                    if (DateTime.Now.Subtract(mute.MutedAt).TotalMilliseconds <= mute.MuteLength)
                    {
                        return;
                    }

                    Console.WriteLine($"Mute on {mute.UserId} for {mute.MuteLength} hour/s is up.");

                    try
                    {
                        var guild = await (_client as IDiscordClient).GetGuildAsync(mute.GuildId);

                        Console.WriteLine($"Guild: {guild.Name}");

                        var user = await guild.GetUserAsync(mute.UserId);

                        Console.WriteLine($"User: {user.Username}");

                        var dbGuild = await _guildRepository.GetGuildAsync(guild.Id);

                        Console.WriteLine("Found guild database.");

                        var mutedRole = guild.GetRole(dbGuild.MutedRoleId);

                        Console.WriteLine($"Muted role: {mutedRole.Name}.");

                        await user.RemoveRoleAsync(mutedRole);

                        Console.WriteLine("Removed muted role.");

                        await _moderationService.ModLogAsync(dbGuild, guild, "Auto unmute", Configuration.UnmuteColor, string.Empty, null, user);

                        Console.WriteLine("Logged to modlog.");

                        await Task.Delay(500);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.StackTrace);
                    }
                    finally
                    {
                        await _muteRepository.DeleteAsync(mute);

                        Console.WriteLine("Mute deleted.");
                    }
                }
            });
        }
    }
}