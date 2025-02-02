using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnUnEquipAffaiblissement(ModuleEvents.OnPlayerUnequipItem onUnequip)
    {
      NwCreature oPC = onUnequip.UnequippedBy;
      NwItem oItem = onUnequip.Item;

      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      await NwTask.NextFrame();

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = oPC.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.HeavyFlail, BaseItemType.Rapier, BaseItemType.Whip, BaseItemType.Sling))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.Rapier, BaseItemType.Whip)))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseAffaiblissement))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseAffaiblissement, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseAffaiblissement, 0);
    }
  }
}
