using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnEquipCheckArmorShieldProficiency(ModuleEvents.OnPlayerUnequipItem onUnequip)
    {
      NwCreature oPC = onUnequip.UnequippedBy;
      NwItem oItem = onUnequip.Item;

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
