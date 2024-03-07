using System;
using System.Collections.Generic;

namespace NightWatcher
{
    public class NightWatcherConfig
    {
        public int EffectRadius = 64;
        public List<string> BlockCodes = new List<string>();        
        public bool BlockDuringStorm = false;
        public bool DebugOutput = false;
        public bool BlockRifts = true;
        public NightWatcherConfig()
        {
            // default radius..
            EffectRadius = 128;                        
            BlockDuringStorm = false;
            DebugOutput = false;
            BlockRifts = true;
        }
    }
}
