using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnEquipMonkUnarmoredDefence(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (oPC is null || oItem is null)
        return;

      switch (oItem.BaseItem.ItemType)
      {
        case BaseItemType.SmallShield:
        case BaseItemType.LargeShield:
        case BaseItemType.TowerShield:
        case BaseItemType.Armor: break;
        default: return;
      }

      NwItem shield = onUnequip.Creature.GetItemInSlot(InventorySlot.RightHand);

      bool hasShield = false;

      if (shield is not null)
        switch (shield.BaseItem.ItemType)
        {
          case BaseItemType.SmallShield:
          case BaseItemType.LargeShield:
          case BaseItemType.TowerShield: hasShield = true; break;
        }

      if (!hasShield && (oPC.GetItemInSlot(InventorySlot.Chest) is null
        || oPC.GetItemInSlot(InventorySlot.Chest).BaseACValue < 1))
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckMonkUnarmoredDefence;
        oPC.OnHeartbeat += CreatureUtils.OnHeartBeatCheckMonkUnarmoredDefence;

        if (oPC.GetAbilityModifier(Ability.Wisdom) > 0)
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkUnarmoredDefenseEffect(oPC.GetAbilityModifier(Ability.Wisdom)));

        if (oPC.Classes.Any(c => c.Class.ClassType == ClassType.Monk && c.Level > 1)
          && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkSpeedEffectTag))
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkSpeedEffect(oPC.Classes.FirstOrDefault(c => c.Class.ClassType == ClassType.Monk).Level));
      }
    }
  }
}
