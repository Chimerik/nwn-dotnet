﻿using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipMutilation(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      NwCreature oPC = onEquip.Player;
      NwItem oItem = onEquip.Item;

      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = oPC.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe, BaseItemType.Battleaxe, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Scythe))
      || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe)))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseMutilation))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMutilation, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMutilation, 0);
    }
  }
}
