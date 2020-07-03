using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Database.Repositories;
using System.Threading.Tasks;
using System.Threading;
using System;

namespace NukoBot.Services.Timers
{
    internal sealed class AutoDeltePolls
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;
        private readonly PollRepository _pollRepository;
        private readonly Text _text;
        private readonly Timer _timer;

        public AutoDeltePolls(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            _pollRepository = _serviceProvider.GetRequiredService<PollRepository>();
            _text = _serviceProvider.GetRequiredService<Text>();

            ObjectState StateObj = new ObjectState();

            TimerCallback TimerDelegate = new TimerCallback(Unmute);

            _timer = new Timer(TimerDelegate, StateObj, TimeSpan.Zero, Configuration.AutoDeletePollsCooldown);

            StateObj.TimerReference = _timer;
        }

        private void Unmute(object stateObj)
        {
            Task.Run(async () =>
            {
                foreach (var poll in await _pollRepository.AllAsync())
                {
                    if (TimeSpan.FromMilliseconds(poll.Length).Subtract(DateTime.Now.Subtract(poll.CreatedAt)).TotalMilliseconds < 0)
                    {
                        var message = string.Empty;
                        var votes = poll.Votes();

                        for (int x = 0; x < poll.Choices.Length; x++)
                        {
                            var choice = poll.Choices[x];
                            var percentage = (votes[choice] / (double)poll.VotesDocument.ElementCount);

                            if (double.IsNaN(percentage))
                            {
                                percentage = 0;
                            }

                            message += $"{x + 1}. {choice}: {votes[choice]} Votes ({percentage.ToString("P")})\n";
                        }

                        var user = _client.GetUser(poll.CreatorId);
                        var userDm = await user.GetOrCreateDMChannelAsync();

                        await _text.SendAsync(userDm, $"These are the results from your poll **{poll.Name}**\n{message}");

                        await _pollRepository.DeleteAsync(poll);
                    }
                }
            });
        }
    }
}