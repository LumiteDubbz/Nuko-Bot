using System.Collections.Generic;

namespace NukoBot.Common
{
    public partial class HelpResponse
    {
        private List<string> triggerWords = new List<string>();

        public List<string> TriggerWords { get { return triggerWords; } set { triggerWords = value; } }

        public string Response { get; set; }
    }
}
