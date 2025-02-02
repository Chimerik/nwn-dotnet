using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnCommotion(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseCommotion))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseCommotion);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipCommotion;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipCommotion;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipCommotion;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipCommotion;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer, BaseItemType.Sling))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.LightHammer, BaseItemType.Warhammer)))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseCommotion))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCommotion, 100);
      }

      return true;
    }
  }
}
