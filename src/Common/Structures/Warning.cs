using System;

namespace NukoBot.Common.Structures
{
    public sealed class Warning
    {
        public ulong ModeratorId { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public string Reason { get; set; }

        public bool Expired { get; set; }
    }
}
