using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnArretCardiaque(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseArretCardiaque))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseArretCardiaque);

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipArretCardiaque;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipArretCardiaque;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipArretCardiaque;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipArretCardiaque;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightMace, BaseItemType.Club, BaseItemType.Morningstar, BaseItemType.HeavyFlail))
        || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightMace, BaseItemType.Club, BaseItemType.Morningstar)))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseArretCardiaque))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseArretCardiaque, 100);
      }

      return true;
    }
  }
}
