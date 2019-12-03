using MongoDB.Bson;
using System;
using System.Collections.Generic;

namespace NukoBot.Database.Models
{
    public sealed class Poll : Model
    {
        public Poll(string name, ulong creatorId, ulong guildId, string[] choices)
        {
            Name = name;
            CreatorId = creatorId;
            GuildId = guildId;
            Choices = choices;
        }

        public string Name { get; set; }

        public ulong CreatorId { get; set; }

        public ulong GuildId { get; set; }

        public string[] Choices { get; set; } = new string[] { };

        public BsonDocument VotesDocument { get; set; } = new BsonDocument();

        public DateTime CreatedAt { get; set; }

        public double Length { get; set; } = 1;

        public IReadOnlyDictionary<string, int> Votes()
        {
            var votesDictionary = new Dictionary<string, int>();

            for (int i = 0; i < Choices.Length; i++)
            {
                votesDictionary.Add(Choices[i], 0);
            }

            foreach (var vote in VotesDocument)
            {
                votesDictionary[vote.Value.AsString]++;
            }

            return votesDictionary;
        }
    }
}