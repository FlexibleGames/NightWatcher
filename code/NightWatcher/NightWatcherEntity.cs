using System;
using System.Collections.Generic;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.MathTools;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Client;
using Vintagestory.API.Common.Entities;
using Vintagestory.GameContent;
using Vintagestory.API.Util;

namespace NightWatcher
{
    public class NightWatcherEntity : EntityHumanoid
    {
        private ICoreServerAPI sapi;
        private NightWatcherConfig watcher;

        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                sapi.Event.OnTrySpawnEntity += SpawnInterceptor;
                sapi.ModLoader.GetModSystem<ModSystemRifts>(true).OnTrySpawnRift += OnRiftSpawn;
                watcher = api.ModLoader.GetModSystem<NightWatcherMod>(true).NWConfig;
            }
            else
            {
                ICoreClientAPI capi = api as ICoreClientAPI;
                
            }            
        }

        /// <summary>
        /// Blocks Rifts from spawning (if enabled in config) in the effective radii of the watcher.
        /// </summary>
        /// <param name="pos">Position of the potential rift.</param>
        /// <param name="handling">Set to PreventDefault to block rifts.</param>
        private void OnRiftSpawn(BlockPos pos, ref EnumHandling handling)
        {
            if (watcher.BlockRifts)
            {
                double distance = this.ServerPos.DistanceTo(pos.ToVec3d());
                if (distance <= watcher.EffectRadius)
                {
                    handling = EnumHandling.PreventDefault;
                    return;
                }
            }
        }

        /// <summary>
        /// Blocks any entity whos code is in the BlockCodes list in range and (optionally) only when a temporal storm is NOT active.
        /// </summary>
        /// <param name="entityProperties"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="herdId"></param>
        /// <returns></returns>
        public bool SpawnInterceptor(IBlockAccessor blockAccessor, ref EntityProperties entityProperties, Vec3d spawnPosition, long herdId)
        {
            if (watcher.BlockCodes == null || watcher.BlockCodes.Count == 0) return true; // nothing to block

            if (watcher.BlockCodes.Contains(entityProperties.Code.FirstCodePart()))
            {
                bool storming = this.Api.ModLoader.GetModSystem<SystemTemporalStability>(true).StormData.nowStormActive;
                bool stormblock = watcher.BlockDuringStorm;
                if (storming && !stormblock) 
                { 
                    if (watcher.DebugOutput)
                    {
                        sapi.Logger.Debug($"Nightwatcher: Storm is active, Storm Block is {stormblock}");
                    }
                    return true;
                }
                
                double distance = this.ServerPos.DistanceTo(spawnPosition);
                if (distance <= watcher.EffectRadius)
                {
                    if (watcher.DebugOutput)
                    {
                        sapi.Logger.Debug($"Nightwatcher: Blocking {entityProperties.Code} at {distance:N0} blocks away.");
                    }
                    return false;
                }
            }
            return true;
        }

        public override void OnInteract(EntityAgent byEntity, ItemSlot slot, Vec3d hitPosition, EnumInteractMode mode)
        {
            if (!this.Alive || this.World.Side == EnumAppSide.Client || mode == EnumInteractMode.Attack)
            {
                base.OnInteract(byEntity, slot, hitPosition, mode);
                return;
            }
            string @string = this.WatchedAttributes.GetString("ownerUid", null);
            EntityPlayer entityPlayer = byEntity as EntityPlayer;
            string text = (entityPlayer != null) ? entityPlayer.PlayerUID : null;
            if (text != null && (@string == null || @string == "" || @string == text) && byEntity.Controls.Sneak)
            {
                ItemStack itemStack = new ItemStack(byEntity.World.GetItem(new AssetLocation("nightwatcher")), 1);
                if (!byEntity.TryGiveItemStack(itemStack))
                {
                    byEntity.World.SpawnItemEntity(itemStack, this.ServerPos.XYZ, null);
                }
                if (Api.Side == EnumAppSide.Server)
                {
                    sapi.Event.OnTrySpawnEntity -= SpawnInterceptor;
                    sapi.ModLoader.GetModSystem<ModSystemRifts>(true).OnTrySpawnRift -= OnRiftSpawn;
                }
                        
                this.Die(EnumDespawnReason.Death, null);
                return;
            }
            base.OnInteract(byEntity, slot, hitPosition, mode);
        }

    }
}
