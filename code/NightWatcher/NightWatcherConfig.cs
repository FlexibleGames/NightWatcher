using System;
using System.Collections.Generic;

namespace NightWatcher
{
    public class NightWatcherConfig
    {
        public int EffectRadius = 64;
        public List<string> BlockCodes = new List<string>();
        public bool BlockDrifters = true;
        public bool BlockDuringStorm = false;
        public NightWatcherConfig()
        {
            // default radius..
            EffectRadius = 64;
            BlockCodes.Add("drifter");
            BlockCodes.Add("bear");
            BlockCodes.Add("wolf");
            BlockDrifters = true;
            BlockDuringStorm = false;
        }
    }
}
