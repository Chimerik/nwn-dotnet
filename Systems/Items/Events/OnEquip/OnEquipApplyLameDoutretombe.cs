using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipApplyLameDoutretombe(OnItemEquip onEquip)
    {
      if (onEquip.Item.BaseItem.ItemType != BaseItemType.TwoBladedSword 
        || onEquip.EquippedBy.ActiveEffects.Any(e => e.Tag == EffectSystem.LameDoutretombeEffectTag))
        return;

      onEquip.EquippedBy.ApplyEffect(EffectDuration.Permanent, EffectSystem.lameDoutretombe);
      onEquip.Item.GetObjectVariable<LocalVariableInt>(ItemConfig.IsFinesseWeaponVariable).Value = 1;
    }
    public static void OnUnEquipRemoveLameDoutretombe(OnItemUnequip onUnEquip)
    {
      NwCreature oPC = onUnEquip.Creature;
      NwItem weapon = onUnEquip.Item;

      if (onUnEquip.Item.BaseItem.ItemType != BaseItemType.TwoBladedSword || !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.AmbiMasterEffectTag))
        return;

      EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.LameDoutretombeEffectTag);
    }
  }
}
