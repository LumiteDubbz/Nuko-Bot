using Discord.Commands;
using NukoBot.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NukoBot.Modules.System
{
    public partial class System
    {
        [Command("Credits")]
        [Alias("creds", "authors", "creators", "makers", "contributors", "owners")]
        [Summary("View the owners of this bot.")]
        public async Task Credits()
        {
            var authors = new List<string>();

            foreach (var author in Configuration.Authors)
            {
                authors.Add(author);
            }

            var message = "A huge and special thanks to the people who helped make this bot:\n";
            var separator = ", ";

            foreach (var author in authors)
            {
                message += $"**{author}**" + separator;
            }

            await SendAsync(message.Remove(message.Length - separator.Length) + ".");
        }
    }
}
