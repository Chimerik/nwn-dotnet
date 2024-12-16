﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMoulinet(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseMoulinet))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseMoulinet);

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipMoulinet;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipMoulinet;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipMoulinet;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipMoulinet;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Rapier, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
        || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Rapier, BaseItemType.Shortsword, BaseItemType.Dagger, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseMoulinet))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseMoulinet, 100);
      }

      return true;
    }
  }
}