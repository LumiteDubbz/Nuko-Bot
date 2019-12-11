using Discord.Commands;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using NukoBot.Common;
using NukoBot.Common.Structures;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NukoBot.Modules.BotOwner
{
    public partial class BotOwner
    {
        [Command("Evaluate")]
        [Alias("eval", "evalcode", "runcode")]
        [Summary("Evaluates given C# scripts.")]
        [Remarks("Configuration.Version")]
        public async Task Evaluate([Remainder] string code)
        {
            try
            {
                var result = await EvaluateAsync(Context, code);

                if (result != null && result.ReturnValue != null && !string.IsNullOrWhiteSpace(result.ReturnValue.ToString()))
                {
                    await SendAsync(result.ReturnValue.ToString());
                }
                else
                {
                    await SendAsync("Success.");
                }
            }
            catch (Exception exception)
            {
                await ReplyErrorAsync(string.Concat("**", exception.GetType().ToString(), "**: ", exception.Message));
            }
        }

        public async Task<ScriptState<object>> EvaluateAsync(Context context, string code)
        {
            var globals = new Globals(context, _guildRepository, _userRepository);
            var scriptOptions = ScriptOptions.Default.WithImports("System", "System.Collections.Generic", "System.Threading.Tasks", "Discord", "Discord.WebSocket", "Discord.Commands", "NukoBot").WithReferences(AppDomain.CurrentDomain.GetAssemblies().Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location)));
            var script = CSharpScript.Create(code, scriptOptions, typeof(Globals));

            script.Compile();

            var result = await script.RunAsync(globals);

            return result;
        }
    }
}
