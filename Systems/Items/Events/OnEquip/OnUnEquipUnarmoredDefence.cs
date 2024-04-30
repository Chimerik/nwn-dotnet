using System.Linq;
using System.Security.Cryptography;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnUnEquipUnarmoredDefence(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;

      if (oPC is null || oItem is null)
        return;

      if(oItem.BaseItem.ItemType == BaseItemType.Armor && (oPC.GetItemInSlot(InventorySlot.Chest) is null
        || oPC.GetItemInSlot(InventorySlot.Chest).BaseACValue < 1))
      {
        oPC.OnHeartbeat -= CreatureUtils.OnHeartBeatCheckUnarmoredDefence;
        oPC.OnHeartbeat += CreatureUtils.OnHeartBeatCheckUnarmoredDefence;

        if (oPC.GetAbilityModifier(Ability.Constitution) > 0 && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.UnarmoredDefenceEffectTag))
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetUnarmoredDefenseEffect(oPC.GetAbilityModifier(Ability.Constitution)));

        if (oPC.Classes.Any(c => c.Class.Id == CustomClass.Barbarian && c.Level > 4)
            && !oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.BarbarianSpeedEffectTag))
          oPC.ApplyEffect(EffectDuration.Permanent, EffectSystem.BarbarianSpeed);
      }
    }
  }
}
