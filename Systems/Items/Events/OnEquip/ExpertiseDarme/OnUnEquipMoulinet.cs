using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnUnEquipMoulinet(ModuleEvents.OnPlayerUnequipItem onUnequip)
    {
      NwCreature oPC = onUnequip.UnequippedBy;
      NwItem oItem = onUnequip.Item;
      
      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      await NwTask.NextFrame();

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = oPC.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Rapier, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
      || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Rapier, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseMoulinet))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMoulinet, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMoulinet, 0);
    }
  }
}
