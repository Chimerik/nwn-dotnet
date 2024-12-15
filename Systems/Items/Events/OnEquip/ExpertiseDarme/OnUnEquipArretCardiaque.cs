using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnUnEquipArretCardiaque(OnItemUnequip onUnequip)
    {
      NwCreature oPC = onUnequip.Creature;
      NwItem oItem = onUnequip.Item;
      
      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      await NwTask.NextFrame();

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = oPC.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightMace, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.HeavyFlail))
      || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightMace, BaseItemType.Club, BaseItemType.Morningstar)))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseArretCardiaque))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseArretCardiaque, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseArretCardiaque, 0);
    }
  }
}
