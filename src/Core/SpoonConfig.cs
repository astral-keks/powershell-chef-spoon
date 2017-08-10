using System.Collections.Generic;

namespace AstralKeks.ChefSpoon.Core
{
    public class SpoonConfig
    {
        public string KnifeExe { get; set; }

        public string KnifeConfig { get; set; }

        public string[] Commands { get; set; }

        public string[] Verbs { get; set; }

        public Dictionary<string, string[]> Arguments { get; set; }
    }
}
