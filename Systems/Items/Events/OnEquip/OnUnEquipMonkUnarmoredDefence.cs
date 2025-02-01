using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnUnEquipMonkUnarmoredDefence(ModuleEvents.OnPlayerUnequipItem onUnequip)
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
        case BaseItemType.Armor: break;
        default: return;
      }

      await NwTask.NextFrame();

      NwItem armor = oPC.GetItemInSlot(InventorySlot.Chest);
      NwItem shield = oPC.GetItemInSlot(InventorySlot.LeftHand);

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
        EffectSystem.ApplyMonkUnarmoredDefenseEffect(oPC);

        if (oPC.GetObjectVariable<LocalVariableInt>("_MONK_SPEED_DISABLED").HasNothing
          && oPC.Classes.Any(c => c.Class.Id == CustomClass.Monk && c.Level > 1)
          && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkSpeedEffectTag))
        {
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetMonkSpeedEffect(oPC.Classes.FirstOrDefault(c => c.Class.Id == CustomClass.Monk).Level));
        }
      }
      else
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckMonkUnarmoredDefence;
        EffectUtils.RemoveTaggedEffect(oPC, EffectSystem.MonkUnarmoredDefenceEffectTag, EffectSystem.MonkSpeedEffectTag);
      }
    }
  }
}
