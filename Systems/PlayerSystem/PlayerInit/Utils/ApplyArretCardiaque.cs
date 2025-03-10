﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyArretCardiaque()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseArretCardiaque))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipArretCardiaque;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipArretCardiaque;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipArretCardiaque;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipArretCardiaque;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightMace, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.HeavyFlail))
          || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightMace, BaseItemType.Club, BaseItemType.Morningstar)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseArretCardiaque))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseArretCardiaque, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseArretCardiaque, 0);
        }
      }
    }
  }
}
