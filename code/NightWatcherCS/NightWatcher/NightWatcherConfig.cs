using System;

namespace NightWatcher
{
    public class NightWatcherConfig
    {
        public int EffectRadius = 64;
        public bool BlockWolves = false;
        public bool BlockDrifters = true;
        public bool BlockDuringStorm = false;
        public NightWatcherConfig()
        {
            // default radius..
            EffectRadius = 64;
            BlockWolves = false;
            BlockDrifters = true;
            BlockDuringStorm = false;
        }
    }
}
