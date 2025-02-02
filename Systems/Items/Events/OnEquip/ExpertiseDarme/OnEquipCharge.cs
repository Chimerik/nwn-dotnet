using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipCharge(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      NwCreature oPC = onEquip.Player;
      NwItem oItem = onEquip.Item;

      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Bastardsword, BaseItemType.Katana, BaseItemType.Longsword, BaseItemType.Halberd, BaseItemType.ShortSpear))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseCharge))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCharge, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCharge, 0);
    }
  }
}
