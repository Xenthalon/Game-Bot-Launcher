using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameLauncher
{
    public class BotState
    {
        public bool InGame { get; set; }
        public uint MapSeed { get; set; }
        public string CharacterName { get; set; }
    }
}
