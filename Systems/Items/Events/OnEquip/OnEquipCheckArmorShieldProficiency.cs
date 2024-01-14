using System.Collections.Generic;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipCheckArmorShieldProficiency(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;
      NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      ModuleSystem.Log.Info($"Equipped : {oItem.Name} - {oItem.BaseItem.ItemType}");

      if (oPC is null || oItem is null)
        return;

      if (swappedItem is not null)
      {

        ModuleSystem.Log.Info($"Swapped : {swappedItem.Name} - {swappedItem.BaseItem.ItemType}");

        switch (swappedItem.BaseItem.ItemType)
        {
          case BaseItemType.SmallShield:
          case BaseItemType.LargeShield:
          case BaseItemType.TowerShield: 
            
            if(!ItemUtils.CheckArmorShieldProficiency(oPC, oPC.GetItemInSlot(InventorySlot.Chest)))
            {
              oPC.OnSpellAction -= SpellSystem.NoArmorShieldProficiencyOnSpellInput;
              EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.ShieldArmorDisadvantageEffectTag);
            }

            break;

          case BaseItemType.Armor:

            if(!ItemUtils.CheckArmorShieldProficiency(oPC, oPC.GetItemInSlot(InventorySlot.LeftHand)))
            {
              oPC.OnSpellAction -= SpellSystem.NoArmorShieldProficiencyOnSpellInput;
              EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.ShieldArmorDisadvantageEffectTag);
            }

            oPC.OnHeartbeat -= OnHeartbeatCheckHeavyArmorSlow;
            EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.heavyArmorSlowEffectTag);

            break;
        }
      }

      switch (oItem.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:
        case BaseItemType.Armor: ItemUtils.CheckArmorShieldProficiency(oPC, oItem); break;
      }
    }
  }
}
