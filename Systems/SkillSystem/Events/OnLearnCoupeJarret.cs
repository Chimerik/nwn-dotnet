﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnCoupeJarret(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseCoupeJarret))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseCoupeJarret);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipCoupeJarret;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipCoupeJarret;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipCoupeJarret;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipCoupeJarret;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortbow, BaseItemType.Longbow, BaseItemType.ThrowingAxe, BaseItemType.Dart))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseCoupeJarret))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCoupeJarret, 100);
      }

      return true;
    }
  }
}
