using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NukoBot.Common;
using NukoBot.Events;
using NukoBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NukoBot.Database.Repositories;

namespace NukoBot
{
    public class Program
    {
        static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        private async Task StartAsync()
        {
            Credentials credentials;

            try
            {
                credentials = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("src")) + "src/Credentials.json"));
            }
            catch(IOException error)
            {
                Console.WriteLine("An exception occurred while loading Credentials.json: " + error.ToString());
                return;
            }

            var client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Info
            });

            var commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async,
                LogLevel = LogSeverity.Info
            });

            var serviceManager = new ServiceManager(client, commandService, credentials);

            var serviceProvider = serviceManager.ServiceProvider;

            serviceProvider.GetRequiredService<MessageReceived>();
            serviceProvider.GetRequiredService<Ready>();
            serviceProvider.GetRequiredService<UserJoined>();

            var runRepository = serviceProvider.GetRequiredService<RunRepository>();

            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);

            await client.LoginAsync(TokenType.Bot, credentials.Token);
            await client.StartAsync();

            await runRepository.AddRunAsync(DateTime.Now);

            await Task.Delay(-1);
        }
    }
}
