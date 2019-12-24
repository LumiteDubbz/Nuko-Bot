using Discord.Commands;
using NukoBot.Common;
using NukoBot.Common.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.General
{
    public partial class General
    {
        [Command("ServerInfo")]
        [Alias("serverinformation", "guildinfo", "guildinformation")]
        [Summary("View information about the server this command is ran in.")]
        public async Task ServerInfo()
        {
            var nukoInfo = "**Nuko information:**\n\n";

            nukoInfo += $"**Cases:** {Context.DbGuild.CaseNumber}\n";
            
            if (Context.DbGuild.CustomCommands.Any())
            {
                var customCommands = "";

                foreach (var command in Context.DbGuild.CustomCommands)
                {
                    customCommands += $"{Context.DbGuild.Prefix}{command.Name} returns \"{command.Value}\",\n";
                }

                nukoInfo += "**Custom commands:**\n" + customCommands.Remove(customCommands.Length - 2) + "\n";
            }

            nukoInfo += $"**Days until warnings expire:** {Context.DbGuild.DaysUntilWarningExpires}\n";
            
            if (Context.DbGuild.DisabledCommands.Any())
            {
                var disabledCommands = "";

                foreach (var command in Context.DbGuild.DisabledCommands)
                {
                    var foundCommand = _commandService.Commands.SingleOrDefault(x => x.Name.ToLower() == command.ToLower());

                    disabledCommands += foundCommand == null ? null : $"{Context.DbGuild.Prefix}{foundCommand.Name},\n";
                }

                nukoInfo += "**Disabled commands:**\n" + disabledCommands.Remove(disabledCommands.Length - 2) + "\n";
            }

            if (Context.DbGuild.IgnoredChannels.Any())
            {
                var ignoredChannels = "";

                foreach (var channel in Context.DbGuild.IgnoredChannels)
                {
                    try
                    {
                        var foundChannel = Context.Guild.GetChannel(channel);

                        ignoredChannels += $"#{foundChannel.Name},\n";
                    }
                    catch { }
                }

                nukoInfo += "**Ignored channels:**\n" + ignoredChannels.Remove(ignoredChannels.Length - 2) + "\n";
            }

            nukoInfo += $"**Locked down:** {Context.DbGuild.LockedDown.ToString().WithUppercaseFirstCharacter()}\n";
            nukoInfo += $"**Maximum warnings before action:** {Context.DbGuild.MaxmimumWarningsBeforeAction}\n";

            try
            {
                var logChannel = Context.Guild.GetChannel(Context.DbGuild.ModLogChannelId);

                nukoInfo += $"**Mod log channel:** #{logChannel.Name}\n";
            }
            catch
            {
                nukoInfo += "**Mod log channel:** None\n";
            }

            if (Context.DbGuild.ModRoles.Any())
            {
                var modRoles = "";

                foreach (var role in Context.DbGuild.ModRoles)
                {
                    modRoles += $"**@{role.Name}**\n";
                }

                nukoInfo += "**Mod roles:**\n" + modRoles.Remove(modRoles.Length - 2) + "\n";
            }

            try
            {
                var mutedRole = Context.Guild.GetRole(Context.DbGuild.MutedRoleId);

                nukoInfo += $"**Muted role:** {mutedRole.Mention}\n";
            }
            catch
            {
                nukoInfo += "**Muted role:** None\n";
            }

            try
            {
                var newUserRole = Context.Guild.GetRole(Context.DbGuild.NewUserRole);

                nukoInfo += $"**New user role:** {newUserRole.Mention}\n";
            }
            catch
            {
                nukoInfo += "**New user role:** None\n";
            }

            nukoInfo += $"**Overhaul commands enabled:** {Context.DbGuild.OverhaulEnabled.ToString().WithUppercaseFirstCharacter()}\n";
            nukoInfo += $"**Overhaul point multiplier:** {Context.DbGuild.PointMultiplier}x\n";
            nukoInfo += $"**Overhaul points:** {Context.DbGuild.Points}\n";
            nukoInfo += $"**Command prefix:** {Context.DbGuild.Prefix}\n";

            if (Context.DbGuild.RankRoles.Any())
            {
                var rankRoles = "";

                foreach (var role in Context.DbGuild.RankRoles)
                {
                    try
                    {
                        var rankRole = Context.Guild.GetRole(ulong.Parse(role.Name));

                        rankRoles += $"{rankRole.Mention},\n";
                    }
                    catch { }
                }

                nukoInfo += rankRoles == "" ? "**Rank roles:** None\n" : "**Rank roles:**\n" + rankRoles.Remove(rankRoles.Length - 2) + "\n";
            }

            try
            {
                var screenshotChannel = Context.Guild.GetChannel(Context.DbGuild.ScreenshotChannelId);

                nukoInfo += $"**Overhaul screenshot channel:** #{screenshotChannel.Name}\n";
            }
            catch
            {
                nukoInfo += "**Overhaul screenshot channel:** None\n";
            }

            try
            {
                var topThreeRole = Context.Guild.GetRole(Context.DbGuild.TopThreeRole);

                nukoInfo += $"**Overhaul top {Configuration.MaximumLeaderboardPosition} role:** {topThreeRole.Mention}\n";
            }
            catch
            {
                nukoInfo += $"**Overhaul top {Configuration.MaximumLeaderboardPosition} role:** None\n";
            }

            nukoInfo += $"**Automatic mute length:** {Context.DbGuild.WarnMuteLength}h\n";
            nukoInfo += $"**Warning punishment:** {Context.DbGuild.WarnPunishment.WithUppercaseFirstCharacter()}\n";
            nukoInfo += $"**Welcome message:** {Context.DbGuild.WelcomeMessage}\n";

            await SendAsync($"**Discord server information:\n\nName:** {Context.Guild.Name}\n**ID:** {Context.Guild.Id}\n**Created at:** {Context.Guild.CreatedAt.DateTime + Context.Guild.CreatedAt.TimeOfDay} ({Math.Floor((DateTime.Now - Context.Guild.CreatedAt).TotalDays)} days old)\n**Member count:** {Context.Guild.MemberCount}\n**Owner:** {Context.Guild.Owner.Mention} {(nukoInfo == "**Nuko information:**\n\n" ? null : "\n\n" + nukoInfo)}");
        }
    }
}
