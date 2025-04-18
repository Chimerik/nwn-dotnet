﻿using System.Linq;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class PlayerSystem
  {
    public partial class Player
    {
      private void ApplyCommotion()
      {
        if (oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseCommotion))
        {
          oid.OnPlayerEquipItem -= ItemSystem.OnEquipCommotion;
          oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipCommotion;
          oid.OnPlayerEquipItem += ItemSystem.OnEquipCommotion;
          oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipCommotion;

          var weapon = oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
          var secondWeapon = oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

          if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer, BaseItemType.Sling))
          || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.LightHammer, BaseItemType.Warhammer)))
          {
            if (!oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseCommotion))
              oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCommotion, 100);
          }
          else
            oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCommotion, 0);
        }
      }
    }
  }
}
