using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnCharge(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseCharge))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseCharge);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipCharge;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipCharge;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipCharge;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipCharge;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Bastardsword, BaseItemType.Katana, BaseItemType.Longsword, BaseItemType.Halberd, BaseItemType.ShortSpear))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseCharge))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseCharge, 100);
      }

      return true;
    }
  }
}
