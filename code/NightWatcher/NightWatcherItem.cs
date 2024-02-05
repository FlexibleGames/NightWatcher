using System;
using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Util;

namespace NightWatcher
{
    public class NightWatcherItem : Item
    {
        public override void OnHeldInteractStart(ItemSlot slot, EntityAgent byEntity, BlockSelection blockSel, EntitySelection entitySel, bool firstEvent, ref EnumHandHandling handling)
        {
            if (blockSel == null) return;

            IWorldAccessor world = byEntity.World;
            EntityPlayer entityPlayer = byEntity as EntityPlayer;
            IPlayer player = world.PlayerByUid((entityPlayer != null) ? entityPlayer.PlayerUID : null);
            if (!byEntity.World.Claims.TryAccess(player, blockSel.Position, EnumBlockAccessFlags.BuildOrBreak))
            {
                slot.MarkDirty();
                return;
            }
            if (!(byEntity is EntityPlayer) || player.WorldData.CurrentGameMode != EnumGameMode.Creative)
            {
                slot.TakeOut(1);
                slot.MarkDirty();
            }
            EntityProperties entityType = byEntity.World.GetEntityType(new AssetLocation("nightwatcher"));
            Entity entity = byEntity.World.ClassRegistry.CreateEntity(entityType);
            if (entity != null)
            {
                entity.ServerPos.X = (double)((float)(blockSel.Position.X + (blockSel.DidOffset ? 0 : blockSel.Face.Normali.X)) + 0.5f);
                entity.ServerPos.Y = (double)(blockSel.Position.Y + (blockSel.DidOffset ? 0 : blockSel.Face.Normali.Y));
                entity.ServerPos.Z = (double)((float)(blockSel.Position.Z + (blockSel.DidOffset ? 0 : blockSel.Face.Normali.Z)) + 0.5f);
                entity.ServerPos.Yaw = byEntity.SidedPos.Yaw + 3.14159274f;
                if (player != null && player.PlayerUID != null)
                {
                    entity.WatchedAttributes.SetString("ownerUid", player.PlayerUID);                    
                }
                entity.Pos.SetFrom(entity.ServerPos);
                byEntity.World.PlaySoundAt(new AssetLocation("sounds/block/torch"), entity, player, true, 32f, 1f);
                byEntity.World.SpawnEntity(entity);
                handling = EnumHandHandling.PreventDefaultAction;
            }            
        }

        public override WorldInteraction[] GetHeldInteractionHelp(ItemSlot inSlot)
        {
            return new WorldInteraction[]
            {
                new WorldInteraction
                {
                    ActionLangCode = "heldhelp-place",
                    MouseButton = EnumMouseButton.Right
                }
            }.Append(base.GetHeldInteractionHelp(inSlot));
        }
    }
}
