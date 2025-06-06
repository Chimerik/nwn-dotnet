﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnStabilisation(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseStabilisation))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseStabilisation);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipStabilisation;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipStabilisation;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipStabilisation;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipStabilisation;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.HeavyCrossbow, BaseItemType.Longbow, BaseItemType.ThrowingAxe))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseStabilisation))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseStabilisation, 100);
      }

      return true;
    }
  }
}
