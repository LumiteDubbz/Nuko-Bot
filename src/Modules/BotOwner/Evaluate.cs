using Discord.Commands;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using NukoBot.Common.Structures;
using System.Threading.Tasks;

namespace NukoBot.Modules.BotOwner
{
    public partial class BotOwner
    {
        [Command("Evaluate")]
        [Alias("eval", "evalcode", "runcode")]
        [Summary("Evaluates given C# scripts.")]
        [Remarks("Configuration.Token")]
        public async Task Evaluate([Remainder] string code)
        {
            var script = CSharpScript.Create(code, globalsType: typeof(Globals));

            if (!_evaluationService.TryCompile(script, out string errorMessage))
            {
                await ReplyErrorAsync($"one or more errors occurred while compiling that script:\n\n**Code:**\n```cs\n{code}```\n\n**Error(s):**\n```{errorMessage}```");
                return;
            }
            else
            {
                var result = await _evaluationService.EvaluateAsync(Context.Guild, script);

                if (result.Success)
                {
                    await SendAsync($"**Code:**\n```cs\n{code}```\n\n**Result:**\n```{result.Result}```");
                }
                else
                {
                    await ReplyErrorAsync($"one or more runtime errors occurred:\n\n**Code:**\n```cs\n{code}```\n\n**Error(s):**\n```{result.Exception}```");
                }
            }
        }
    }
}
