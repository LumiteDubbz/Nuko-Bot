using NukoBot.Database.Repositories;

namespace NukoBot.Common.Structures
{
    public sealed class Globals
    {
        public Context Context { get; set; }
        public GuildRepository GuildRepository { get; set; }
        public UserRepository UserRepository { get; set; }

        public Globals(Context context, GuildRepository guildRepository, UserRepository userRepository)
        {
            Context = context;
            GuildRepository = guildRepository;
            UserRepository = userRepository;
        }
    }
}
