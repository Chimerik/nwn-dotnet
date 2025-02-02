using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFrappeDuPommeau(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseFrappeDuPommeau))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseFrappeDuPommeau);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipFrappeDuPommeau;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipFrappeDuPommeau;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipFrappeDuPommeau;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipFrappeDuPommeau;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Longsword, BaseItemType.Bastardsword, BaseItemType.Greatsword))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseFrappeDuPommeau))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFrappeDuPommeau, 100);
      }

      return true;
    }
  }
}
