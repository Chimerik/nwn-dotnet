﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTranspercer(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseTranspercer))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseTranspercer);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipTranspercer;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipTranspercer;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipTranspercer;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipTranspercer;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Dagger, BaseItemType.ShortSpear, BaseItemType.Rapier, BaseItemType.Shortsword))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Dagger, BaseItemType.ShortSpear, BaseItemType.Rapier, BaseItemType.Shortsword)))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseTranspercer))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTranspercer, 100);
      }

      return true;
    }
  }
}
