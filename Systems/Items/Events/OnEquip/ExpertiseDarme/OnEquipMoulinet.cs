﻿using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public partial class ItemSystem
  {
    public static void OnEquipMoulinet(OnItemEquip onEquip)
    {
      NwCreature oPC = onEquip.EquippedBy;
      NwItem oItem = onEquip.Item;
      //NwItem swappedItem = oPC.GetItemInSlot(onEquip.Slot);

      if (oPC is null || oItem is null || !ItemUtils.IsWeapon(oItem.BaseItem))
        return;

      if (Utils.In(oItem.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer, BaseItemType.Sling))
      {
        if (!oPC.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseMoulinet))
          oPC.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMoulinet, 100);
      }
      else
      {
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
}
