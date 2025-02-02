using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnPreparation(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertisePreparation))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertisePreparation);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipPreparation;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipPreparation;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipPreparation;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipPreparation;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.Battleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Katana, BaseItemType.Scythe))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertisePreparation))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertisePreparation, 100);
      }

      return true;
    }
  }
}
