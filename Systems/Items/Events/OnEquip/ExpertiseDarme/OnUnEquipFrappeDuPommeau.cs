using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static async void OnUnEquipFrappeDuPommeau(ModuleEvents.OnPlayerUnequipItem onUnequip)
    {
      NwCreature oPC = onUnequip.UnequippedBy;
      NwItem oItem = onUnequip.Item;
      
      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      await NwTask.NextFrame();

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Longsword, BaseItemType.Bastardsword, BaseItemType.Greatsword))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseFrappeDuPommeau))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFrappeDuPommeau, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFrappeDuPommeau, 0);
    }
  }
}
