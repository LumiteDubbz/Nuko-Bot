using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Common.Extensions;
using System;
using System.Threading.Tasks;

namespace NukoBot.Services
{
    public sealed class AutomaticHelpService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Text _text;

        public AutomaticHelpService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _text = _serviceProvider.GetRequiredService<Text>();
        }

        public HelpResponse HasTriggerWord(SocketMessage message)
        {
            foreach (var helpResponse in Configuration.AutomaticHelpResponses)
            {
                //foreach (var triggerWord in helpResponse.TriggerWords)
                //{
                //    var similarity = StringExtension.CalculateStringSimilarity(message.Content.ToLower(), triggerWord);

                //    if (similarity >= 0.7)
                //    {
                //        return helpResponse;
                //    }
                //}
            }

            return null;
        }

        public async Task HandleHelpRequestAsync(Context context, HelpResponse helpResponse)
        {
            await _text.SendAsync(context.Channel, "Not implemented yet.");
            //await _text.SendAsync(context.Channel, helpResponse.Response);
        }
    }
}
