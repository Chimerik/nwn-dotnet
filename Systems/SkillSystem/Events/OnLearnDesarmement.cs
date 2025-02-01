using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDesarmement(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseDesarmement))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseDesarmement);

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipExpertiseDesarmement;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipDesarmement;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipExpertiseDesarmement;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipDesarmement;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Whip))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseDesarmement))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseDesarmement, 100);
      }

      return true;
    }
  }
}
