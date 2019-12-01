using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using NukoBot.Common;
using NukoBot.Database.Repositories;
using NukoBot.Services;
using System;
//using System.Net;
//using Newtonsoft.Json;

namespace NukoBot.Modules.General
{
    [Name("General")]
    [Summary("General commands not directly related to the core functionality of the bot.")]
    [RequireContext(ContextType.Guild)]
    public partial class General : Module
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly UserRepository _userRepository;
        private readonly PollRepository _pollRepository;
        private readonly PointService _pointService;

        public General(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _userRepository = _serviceProvider.GetRequiredService<UserRepository>();
            _pollRepository = _serviceProvider.GetRequiredService<PollRepository>();
            _pointService = _serviceProvider.GetRequiredService<PointService>();
        }

        //[Command("Define")]
        //[Alias("dictionary")]
        //[Summary("Search for the definition of a word.")]
        //public async Task Define([Summary("The word/s you want to search for.")][Remainder] string word)
        //{
        //    var webClient = new WebClient();
        //    var url = $"https://dictionaryapi.com/api/v3/references/collegiate/json/{word}?key=aaaa";
        //    var file = webClient.DownloadString(url);
        //}
    }
}