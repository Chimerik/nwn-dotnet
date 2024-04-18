using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipMonkUnarmoredDefence(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;
      NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null || (onEquip.Slot != InventorySlot.Chest && onEquip.Slot != InventorySlot.RightHand))
        return;

      switch (oItem.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield: return;;
      }

      if (oItem.BaseACValue < 1)
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckMonkUnarmoredDefence;
        oPC.OnHeartbeat += CreatureUtils.OnHeartBeatCheckMonkUnarmoredDefence;

        if (oPC.GetAbilityModifier(Ability.Wisdom) > 0 && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkUnarmoredDefenceEffectTag))
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkUnarmoredDefenseEffect(oPC.GetAbilityModifier(Ability.Wisdom)));

        if (oPC.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 1)
          && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkSpeedEffectTag))
        {
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkSpeedEffect(oPC.Classes.FirstOrDefault(c => c.Class.Id == CustomClass.Monk).Level));
        }
      }
    }
  }
}
