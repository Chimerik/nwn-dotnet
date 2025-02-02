using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnEraflure(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseEraflure))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseEraflure);


      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipEraflure;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipEraflure;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipEraflure;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipEraflure;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.HeavyFlail, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.LightFlail, BaseItemType.HeavyFlail, BaseItemType.Morningstar, BaseItemType.LightMace, BaseItemType.DireMace, BaseItemType.LightHammer, BaseItemType.Warhammer)))
      {
        player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackEraflure;
        player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackEraflure;
      }

      return true;
    }
  }
}
