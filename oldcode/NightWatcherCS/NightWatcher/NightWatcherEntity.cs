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

        public override void Initialize(EntityProperties properties, ICoreAPI api, long InChunkIndex3d)
        {
            base.Initialize(properties, api, InChunkIndex3d);
            if (api.Side == EnumAppSide.Server)
            {
                sapi = api as ICoreServerAPI;
                sapi.Event.OnTrySpawnEntity += SpawnInterceptor;
            }
            else
            {
                ICoreClientAPI capi = api as ICoreClientAPI;
                
            }
        }

        private NightWatcherMod NightWatcher
        {
            get
            {
                return this.Api.ModLoader.GetModSystem<NightWatcherMod>();
            }
        }

        /// <summary>
        /// Only block drifters (and optionally wolves) in range and (optionally) only when a temporal storm is NOT active.
        /// </summary>
        /// <param name="entityProperties"></param>
        /// <param name="spawnPosition"></param>
        /// <param name="herdId"></param>
        /// <returns></returns>
        public bool SpawnInterceptor(IBlockAccessor blockAccessor, ref EntityProperties entityProperties, Vec3d spawnPosition, long herdId)
        {
            if (NightWatcher.NWConfig.BlockWolves && entityProperties.Code.Path.StartsWithFast("wolf"))
            {
                double distance = this.ServerPos.DistanceTo(spawnPosition);
                if (distance <= NightWatcher.NWConfig.EffectRadius)
                {
                    return false;
                }
            }
            if (NightWatcher.NWConfig.BlockBears && entityProperties.Code.Path.StartsWithFast("bear"))
            {
                double distance = this.ServerPos.DistanceTo(spawnPosition);
                if (distance <= NightWatcher.NWConfig.EffectRadius)
                {
                    return false;
                }
            }

            if (NightWatcher.NWConfig.BlockDrifters && entityProperties.Code.Path.StartsWithFast("drifter"))
            {
                if (NightWatcher.NWConfig.BlockDuringStorm && this.Api.ModLoader.GetModSystem<SystemTemporalStability>().StormData.nowStormActive)
                {
                    // trying to spawn a drifter and no storm is active. Check position
                    double distance = this.ServerPos.DistanceTo(spawnPosition);

                    if (distance <= NightWatcher.NWConfig.EffectRadius)
                    {
                        return false;
                    }
                }
                else if (!this.Api.ModLoader.GetModSystem<SystemTemporalStability>().StormData.nowStormActive)
                {
                    double distance = this.ServerPos.DistanceTo(spawnPosition);

                    if (distance <= NightWatcher.NWConfig.EffectRadius)
                    {
                        return false;
                    }
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
                }
                        
                this.Die(EnumDespawnReason.Death, null);
                return;
            }
            base.OnInteract(byEntity, slot, hitPosition, mode);
        }

    }
}
