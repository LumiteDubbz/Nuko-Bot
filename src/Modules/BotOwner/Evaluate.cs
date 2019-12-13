using Discord.Commands;
using System;
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
                var result = await _evaluationService.EvaluateAsync(Context, code);

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
    }
}
