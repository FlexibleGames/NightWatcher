using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace NightWatcher
{
    public class NightWatcherMod : ModSystem
    {
        ICoreAPI api;
        public NightWatcherConfig NWConfig
        {
            get
            {
                return (NightWatcherConfig)this.api.ObjectCache["watcher_config.json"];
            }
            set
            {
                this.api.ObjectCache.Add("watcher_config.json", value);
            }
        }
        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            this.api = api;
            api.RegisterItemClass("NightWatcherItem", typeof(NightWatcherItem));
            api.RegisterEntity("NightWatcherEntity", typeof(NightWatcherEntity));
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            NightWatcherConfig watcherConfig = null;
            try
            {
                watcherConfig = api.LoadModConfig<NightWatcherConfig>("watcher_config.json");
            }
            catch (Exception)
            {
                api.Logger.Warning("NightWatcher: Config Exception! Config will be rebuilt.");
            }
            if (watcherConfig == null)
            {
                api.Logger.Warning("NightWatcher: Config Error! A typo or a new config setting can cause this. Config will be rebuilt.");
                NightWatcherConfig nwc = new NightWatcherConfig();
                nwc.BlockCodes.Add("drifter");
                nwc.BlockCodes.Add("bear");
                nwc.BlockCodes.Add("wolf");
                api.StoreModConfig<NightWatcherConfig>(nwc, "watcher_config.json");
                watcherConfig = api.LoadModConfig<NightWatcherConfig>("watcher_config.json");
            }
            this.NWConfig = watcherConfig;
        }
    }
}
