using Discord;
using System;
using System.Threading.Tasks;

namespace NukoBot.Services
{
    public sealed class Logger
    {
        public Task LogAsync(LogSeverity severity, string message)
        {
            return Console.Out.WriteLineAsync($"{DateTime.Now.ToString("hh:mm:ss")} [{severity}] {message}");
        }
    }
}
