using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Systems.Arena;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipTranspercer(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;
      //NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      if (Utils.In(oItem.BaseItem.ItemType, BaseItemType.Dagger, BaseItemType.ShortSpear, BaseItemType.Rapier, BaseItemType.Shortsword))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseTranspercer))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTranspercer, 100);
      }
      else
      {
        var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);
        var secondWeapon = oPC.GetItemInSlot(InventorySlot.LeftHand);

        if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Dagger, BaseItemType.ShortSpear, BaseItemType.Rapier, BaseItemType.Shortsword))
        || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Dagger, BaseItemType.ShortSpear, BaseItemType.Rapier, BaseItemType.Shortsword)))
        {
          if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseTranspercer))
            oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTranspercer, 100);
        }
        else
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTranspercer, 0);
      }
    }
  }
}
