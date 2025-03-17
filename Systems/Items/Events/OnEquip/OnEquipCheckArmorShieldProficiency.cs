using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipCheckArmorShieldProficiency(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      NwCreature oPC = onEquip.Player;
      NwItem oItem = onEquip.Item;
      NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null)
        return;

      if (swappedItem is not null)
      {
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
