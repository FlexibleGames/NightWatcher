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
            NightWatcherConfig watcherConfig = api.LoadModConfig<NightWatcherConfig>("watcher_config.json");
            if (watcherConfig == null)
            {
                base.Mod.Logger.Warning("Regenerating default config as it was missing or broken...");
                api.StoreModConfig<NightWatcherConfig>(new NightWatcherConfig(), "watcher_config.json");
                watcherConfig = api.LoadModConfig<NightWatcherConfig>("watcher_config.json");
            }
            this.NWConfig = watcherConfig;
        }
    }
}
