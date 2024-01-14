using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnEquipCheckArmorShieldProficiency(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      ModuleSystem.Log.Info($"unequipped : {oItem.Name} - {oItem.BaseItem.ItemType}");

      if (oPC is null || oItem is null)
        return;

      switch (oItem.BaseItem.ItemType) 
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:

          if (!ItemUtils.CheckArmorShieldProficiency(oPC, oPC.GetItemInSlot(InventorySlot.Chest)))
          {
            oPC.OnSpellAction -= SpellSystem.NoArmorShieldProficiencyOnSpellInput;
            EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.ShieldArmorDisadvantageEffectTag);
          }

          break;

        case BaseItemType.Armor:

          if (!ItemUtils.CheckArmorShieldProficiency(oPC, oPC.GetItemInSlot(InventorySlot.LeftHand)))
          {
            oPC.OnSpellAction -= SpellSystem.NoArmorShieldProficiencyOnSpellInput;
            EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.ShieldArmorDisadvantageEffectTag);
          }

          oPC.OnHeartbeat -= OnHeartbeatCheckHeavyArmorSlow;
          EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.heavyArmorSlowEffectTag);

          break;
      }
    }
  }
}
