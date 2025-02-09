using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDestabiliser(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseDestabiliser))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseDestabiliser);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipDestabiliser;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipDestabiliser;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipDestabiliser;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipDestabiliser;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Quarterstaff, BaseItemType.MagicStaff, BaseItemType.Whip))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseDestabiliser))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDestabiliser, 100);
      }
      else
        player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDestabiliser, 0);

      return true;
    }
  }
}
