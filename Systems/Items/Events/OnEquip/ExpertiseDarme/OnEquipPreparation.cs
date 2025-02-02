using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipPreparation(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      NwCreature oPC = onEquip.Player;
      NwItem oItem = onEquip.Item;

      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.Battleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Katana, BaseItemType.Scythe))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertisePreparation))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertisePreparation, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertisePreparation, 0);
    }
  }
}
