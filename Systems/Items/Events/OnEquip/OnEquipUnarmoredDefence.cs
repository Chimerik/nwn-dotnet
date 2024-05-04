using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipUnarmoredDefence(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;
      NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null || onEquip.Slot != InventorySlot.Chest)
        return;

      if (oItem is null || oItem.BaseACValue < 1)
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        oPC.OnHeartbeat += CreatureUtils.OnHeartBeatCheckUnarmoredDefence;

        if (oPC.GetAbilityModifier(Ability.Constitution) > 0 && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.UnarmoredDefenceEffectTag))
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetUnarmoredDefenseEffect(oPC.GetAbilityModifier(Ability.Constitution)));

        if (oPC.Classes.Any(c => c.Class.Id == CustomClass.Barbarian && c.Level > 4)
            && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianSpeedEffectTag))
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.BarbarianSpeed);
      }
      else
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.UnarmoredDefenceEffectTag, EffectSystem.BarbarianSpeedEffectTag);
      }
    }
  }
}
