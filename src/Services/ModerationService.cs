using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Database.Models;
using NukoBot.Database.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Services
{
    public sealed class ModerationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GuildRepository _guildRepository;
        private readonly Text _text;

        public ModerationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _guildRepository = _serviceProvider.GetRequiredService<GuildRepository>();
            _text = _serviceProvider.GetRequiredService<Text>();
        }

        public int GetPermissionLevel(Guild dbGuild, IGuildUser user)
        {
            if (user.Guild.OwnerId == user.Id)
            {
                return 3;
            }

            var permLevel = 0;

            if (dbGuild.ModRoles != 0)
            {
                foreach (var role in dbGuild.ModRoles.OrderBy(x => x.Value))
                {
                    if (user.Guild.GetRole(ulong.Parse(role.Name)) != null)
                    {
                        if (user.RoleIds.Any(x => x.ToString() == role.Name))
                        {
                            permLevel = role.Value.AsInt32;
                        }
                    }
                }
            }

            return user.GuildPermissions.Administrator && permLevel < 2 ? 2 : permLevel;
        }

        public async Task InformUserAsync(SocketUser user, string message)
        {
            var userDm = await user.GetOrCreateDMChannelAsync();

            await _text.SendAsync(userDm, message);
        }

        public async Task ModLogAsync(Guild dbGuild, IGuild guild, string action, Color color, string reason, IGuildUser moderator, IGuildUser user)
        {
            var logChannel = await guild.GetTextChannelAsync(dbGuild.ModLogChannelId);

            if (logChannel == null) return;

            var builder = new EmbedBuilder()
             .WithColor(color)
             .WithFooter(x => {
                 x.Text = $"Case #{dbGuild.CaseNumber}";
             })
             .WithCurrentTimestamp();

            if (moderator != null)
            {
                builder.WithAuthor(x => {
                    x.Name = $"{moderator}";
                });
            }

            var description = $"**Action:** {action}\n";

            if (user != null)
            {
                description += $"**User:** {user}\n";
            }

            if (!string.IsNullOrWhiteSpace(reason))
            {
                description += $"**Reason:** {reason}\n";
            }

            builder.WithDescription(description);

            try
            {
                await logChannel.SendMessageAsync(string.Empty, embed: builder.Build());

                await _guildRepository.ModifyAsync(x => x.Id == dbGuild.Id, x => x.CaseNumber++);
            }
            catch
            {
                return;
            }
        }
    }
}