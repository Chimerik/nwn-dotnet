using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnFendre(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseFendre))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseFendre);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipFendre;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipFendre;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipFendre;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipFendre;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Battleaxe, BaseItemType.Doubleaxe, BaseItemType.Greatsword, BaseItemType.TwoBladedSword, BaseItemType.Greataxe, BaseItemType.DwarvenWaraxe, BaseItemType.Halberd, BaseItemType.Scythe))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Battleaxe, BaseItemType.DwarvenWaraxe)))
      {
        if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseFendre))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFendre, 100);
      }

      return true;
    }
  }
}
