using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMutilation(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseMutilation))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseMutilation);

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipMutilation;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipMutilation;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipMutilation;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipMutilation;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe, BaseItemType.Battleaxe, BaseItemType.Greataxe, BaseItemType.Doubleaxe, BaseItemType.DwarvenWaraxe, BaseItemType.Scythe))
        || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Handaxe)))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseMutilation))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMutilation, 100);
      }

      return true;
    }
  }
}
