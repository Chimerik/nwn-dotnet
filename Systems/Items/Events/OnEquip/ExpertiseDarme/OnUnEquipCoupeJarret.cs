using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnUnEquipCoupeJarret(ModuleEvents.OnPlayerUnequipItem onUnequip)
    {
      NwCreature oPC = onUnequip.UnequippedBy;
      NwItem oItem = onUnequip.Item;
      
      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      await NwTask.NextFrame();

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortbow, BaseItemType.Longbow, BaseItemType.ThrowingAxe, BaseItemType.Dart))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseCoupeJarret))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCoupeJarret, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCoupeJarret, 0);
    }
  }
}
