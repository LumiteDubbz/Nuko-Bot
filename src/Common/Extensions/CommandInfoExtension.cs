using Discord;
using Discord.Commands;

namespace NukoBot.Common.Extensions
{
    public static class CommandInfoExtension
    {
        public static string GetUsage(this CommandInfo command)
        {
            var usage = string.Empty;

            foreach (var parameter in command.Parameters)
            {
                var before = "<";
                var after = ">";

                if (parameter.IsOptional)
                {
                    before = "[";
                    after = "]";
                }

                if (parameter.Type == typeof(IRole) || parameter.Type == typeof(IGuildUser))
                {
                    before += "@";
                }

                if (parameter.Type == typeof(ITextChannel))
                {
                    before += "#";
                }

                usage += $" {before}{parameter.Name}{after}";
            }

            return usage;
        }
    }
}
