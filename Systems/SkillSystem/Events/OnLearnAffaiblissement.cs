using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAffaiblissement(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseAffaiblissement))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseAffaiblissement);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipAffaiblissement;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipAffaiblissement;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipAffaiblissement;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipAffaiblissement;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.HeavyFlail, BaseItemType.Rapier, BaseItemType.Whip, BaseItemType.Sling))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.Rapier, BaseItemType.Whip)))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseAffaiblissement))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseAffaiblissement, 100);
      }

      return true;
    }
  }
}
