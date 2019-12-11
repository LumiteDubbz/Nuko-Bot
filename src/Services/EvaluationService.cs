using Discord;
using Discord.WebSocket;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using NukoBot.Common.Structures;
using NukoBot.Database.Models;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NukoBot.Services
{
    public sealed class EvaluationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly IMongoCollection<User> _users;
        private readonly Text _text;

        public EvaluationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _users = _serviceProvider.GetRequiredService<IMongoCollection<User>>();
            _text = _serviceProvider.GetRequiredService<Text>();
        }

        public bool TryCompile(Script script, out string errorMessage)
        {
            var compiledScript = script.Compile();
            var compilationErrors = compiledScript.Where(x => x.Severity == DiagnosticSeverity.Error);
            var errorMessageBuilder = new StringBuilder();

            foreach (var error in compilationErrors)
            {
                errorMessageBuilder.AppendFormat("{0}\n", error.GetMessage());
            }

            errorMessage = errorMessageBuilder.ToString();

            return errorMessageBuilder.Length == 0;
        }

        public async Task<EvaluationResult> EvaluateAsync(IGuild guild, Script script)
        {
            try
            {
                var scriptResult = await script.RunAsync(new Globals(_client, guild, _text, _users));
                return EvaluationResult.FromSuccess(scriptResult.ReturnValue?.ToString() ?? "Success.");
            } 
            catch(Exception error)
            {
                return EvaluationResult.FromError(error);
            }
        }
    }
}
