﻿using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipEntaille(ModuleEvents.OnPlayerEquipItem onEquip)
    {
      NwCreature oPC = onEquip.Player;
      NwItem oItem = onEquip.Item;

      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      var weapon = oPC.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = oPC.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Handaxe, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.LightHammer, BaseItemType.Kukri, BaseItemType.Sickle))
      || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oPC, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Handaxe, BaseItemType.Scimitar, BaseItemType.LightHammer, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseEntaille))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseEntaille, 100);
      }
      else
        oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseEntaille, 0);
    }
  }
}
