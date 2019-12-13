using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Structures;
using NukoBot.Database.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Services
{
    public sealed class EvaluationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly GuildRepository _guildRepository;
        private readonly UserRepository _userRepository;

        public EvaluationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _guildRepository = _serviceProvider.GetRequiredService<GuildRepository>();
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
        }

        public async Task<ScriptState<object>> EvaluateAsync(Context context, string code)
        {
            var globals = new Globals(context, _guildRepository, _userRepository);
            var scriptOptions = ScriptOptions.Default.WithImports("System", "System.Collections.Generic", "System.Threading.Tasks", "Discord", "Discord.WebSocket", "Discord.Commands", "NukoBot", "NukoBot.Common").WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)));
            var script = CSharpScript.Create(code, scriptOptions, typeof(Globals));

            script.Compile();

            var result = await script.RunAsync(globals);

            return result;
        }
    }
}
