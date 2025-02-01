using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnUnEquipUnarmoredDefence(ModuleEvents.OnPlayerUnequipItem onUnequip)
    {
      NwCreature oPC = onUnequip.UnequippedBy;
      NwItem oItem = onUnequip.Item;
      
      if (oPC is null || oItem is null || oItem.BaseItem.ItemType != BaseItemType.Armor)
        return;

      await NwTask.NextFrame();

      NwItem equippedArmor = oPC.GetItemInSlot(InventorySlot.Chest);

      if (equippedArmor is null || equippedArmor.BaseACValue < 1)
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        oPC.OnHeartbeat += CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        EffectSystem.ApplyUnarmoredDefenseEffect(oPC);

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
