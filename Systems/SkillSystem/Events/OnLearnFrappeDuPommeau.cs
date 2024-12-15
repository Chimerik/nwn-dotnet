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

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipFrappeDuPommeau;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipFrappeDuPommeau;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipFrappeDuPommeau;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipFrappeDuPommeau;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if (weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Longsword, BaseItemType.Bastardsword, BaseItemType.Greatsword))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseFrappeDuPommeau))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseFrappeDuPommeau, 100);
      }

      return true;
    }
  }
}
