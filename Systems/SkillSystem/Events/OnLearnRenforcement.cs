using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRenforcement(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseRenforcement))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseRenforcement);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipRenforcement;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipRenforcement;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipRenforcement;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipRenforcement;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.ShortSpear, BaseItemType.Halberd, BaseItemType.TwoBladedSword))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseRenforcement))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseRenforcement, 100);
      }

      return true;
    }
  }
}
