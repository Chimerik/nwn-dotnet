using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnUnEquipResistanceDraconique(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;
      
      if (oPC is null || oItem is null || oItem.BaseItem.ItemType != BaseItemType.Armor)
        return;

      await NwTask.NextFrame();

      NwItem equippedArmor = oPC.GetItemInSlot(InventorySlot.Chest);

      if (equippedArmor is null || equippedArmor.BaseACValue < 1)
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckResistanceDraconique;
        oPC.OnHeartbeat += CreatureUtils.OnHeartBeatCheckResistanceDraconique;

        if (oPC.GetAbilityModifier(Ability.Constitution) > 0 && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.ResistanceDraconiqueEffectTag))
          EffectSystem.ApplyResistanceDraconiqueEffect(oPC, oPC.GetAbilityModifier(Ability.Charisma));
      }
      else
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckResistanceDraconique;
        EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.ResistanceDraconiqueEffectTag);
      }
    }
  }
}
