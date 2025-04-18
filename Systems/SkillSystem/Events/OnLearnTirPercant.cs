﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTirPercant(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseTirPercant))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseTirPercant);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipTirPercant;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipTirPercant;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipTirPercant;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipTirPercant;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightCrossbow, BaseItemType.HeavyCrossbow, BaseItemType.Shuriken))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseTirPercant))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTirPercant, 100);
      }
      else
        player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseTirPercant, 0);

      return true;
    }
  }
}
