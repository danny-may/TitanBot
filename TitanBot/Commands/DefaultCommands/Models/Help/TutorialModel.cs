using System.Collections.Generic;

namespace TitanBot.Commands
{
    public class TutorialModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Steps { get; set; }
    }
}