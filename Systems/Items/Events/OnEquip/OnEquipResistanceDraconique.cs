using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipResistanceDraconique(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;
      NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null || onEquip.Slot != InventorySlot.Chest)
        return;

      if (oItem is null || oItem.BaseACValue < 1)
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckResistanceDraconique;
        oPC.OnHeartbeat += CreatureUtils.OnHeartBeatCheckResistanceDraconique;

        if (oPC.GetAbilityModifier(Ability.Charisma) > 0 && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.ResistanceDraconiqueEffectTag))
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetResistanceDraconiqueEffect(oPC.GetAbilityModifier(Ability.Charisma)));
      }
      else
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckResistanceDraconique;
        EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.ResistanceDraconiqueEffectTag);
      }
    }
  }
}
