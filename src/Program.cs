using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using NukoBot.Common;
using NukoBot.Services;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using NukoBot.Database.Repositories;

namespace NukoBot
{
    internal class Program
    {
        static void Main() => new Program().StartAsync().GetAwaiter().GetResult();

        private async Task StartAsync()
        {
            Credentials credentials;

            try
            {
                credentials = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.IndexOf("src")) + "src/Credentials.json"));
            }
            catch (IOException error)
            {
                Console.WriteLine("An exception occurred while loading Credentials.json: The src/Credentials.json file was not found or was not properly formatted.\n\n" + error.ToString());
                return;
            }

            Console.WriteLine("Credentials initialised.");

            var client = new DiscordSocketClient();

            Console.WriteLine("Client initialised.");

            var commandService = new CommandService(new CommandServiceConfig
            {
                DefaultRunMode = RunMode.Async
            });

            Console.WriteLine("CommandService initialised.");

            var serviceManager = new ServiceManager(client, commandService, credentials);

            Console.WriteLine("ServiceManager initialised.");

            var serviceProvider = serviceManager.ServiceProvider;

            serviceManager.InitialiseTimersAndEvents();

            Console.WriteLine("Timers and events initialised.");

            await commandService.AddModulesAsync(Assembly.GetEntryAssembly(), serviceProvider);

            Console.WriteLine("Commands initialised.");

            await client.LoginAsync(TokenType.Bot, credentials.Token);
            await client.StartAsync();

            var runRepository = serviceProvider.GetRequiredService<RunRepository>();

            await runRepository.AddRunAsync(DateTime.Now);

            await Task.Delay(-1);
        }
    }
}