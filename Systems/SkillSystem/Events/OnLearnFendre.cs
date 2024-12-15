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

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipFendre;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipFendre;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipFendre;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipFendre;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Doubleaxe, BaseItemType.Greatsword, BaseItemType.TwoBladedSword, BaseItemType.Greataxe, BaseItemType.DwarvenWaraxe, BaseItemType.Halberd, BaseItemType.Scythe))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseFendre))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFendre, 100);
      }

      return true;
    }
  }
}
