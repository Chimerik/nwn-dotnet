using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnBriseEchine(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseBriseEchine))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseBriseEchine);

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipBriseEchine;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipBriseEchine;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipBriseEchine;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipBriseEchine;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.DireMace, BaseItemType.Warhammer))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseBriseEchine))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseBriseEchine, 100);
      }

      return true;
    }
  }
}
