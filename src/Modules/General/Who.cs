using Discord;
using Discord.Commands;
using NukoBot.Common.Extensions;
using System.Threading.Tasks;

namespace NukoBot.Modules.General
{
    public partial class General
    {
        [Command("Who")]
        [Alias("whois", "user", "userinfo")]
        [Summary("View information about a user.")]
        [Remarks("Gibraltar#1044")]
        public async Task Who(IUser user = null)
        {
            user ??= Context.User;

            await SendImageAsync($"**{user.Username}#{user.Discriminator} ({user.Id})\n\nCreated at:** {user.CreatedAt.Date + user.CreatedAt.TimeOfDay}\n**Status:** {user.Status} ({user.Activity})\n**Bot:** {user.IsBot.ToString().WithUppercaseFirstCharacter()}", user.GetAvatarUrl());
        }
    }
}
