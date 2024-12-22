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
      //NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null || (onEquip.Slot != InventorySlot.Chest && onEquip.Slot != InventorySlot.LeftHand))
        return;

      if (oItem.BaseACValue < 1)
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
