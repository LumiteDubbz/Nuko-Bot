using System;

namespace NukoBot.Database.Models
{
    public partial class Run : Model
    {
        public int RunNumber { get; set; } = 0;
        public DateTime TimeCommenced { get; set; }
    }
}
