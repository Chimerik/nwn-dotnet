using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnEntaille(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseEntaille))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseEntaille);

      player.oid.OnPlayerEquipItem -= ItemSystem.OnEquipEntaille;
      player.oid.OnPlayerUnequipItem -= ItemSystem.OnUnEquipEntaille;
      player.oid.OnPlayerEquipItem += ItemSystem.OnEquipEntaille;
      player.oid.OnPlayerUnequipItem += ItemSystem.OnUnEquipEntaille;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, weapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Handaxe, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.LightHammer, BaseItemType.Kukri, BaseItemType.Sickle))
        || (secondWeapon is not null && ItemUtils.IsCreatureWeaponExpert(player.oid.LoginCreature, secondWeapon) && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Handaxe, BaseItemType.Scimitar, BaseItemType.LightHammer, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseEntaille))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseEntaille, 100);
      }

      return true;
    }
  }
}
