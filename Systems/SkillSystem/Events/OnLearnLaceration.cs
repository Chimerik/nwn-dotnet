﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnLaceration(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ExpertiseLaceration))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ExpertiseLaceration);

      player.oid.LoginCreature.OnItemEquip -= ItemSystem.OnEquipLaceration;
      player.oid.LoginCreature.OnItemUnequip -= ItemSystem.OnUnEquipLaceration;
      player.oid.LoginCreature.OnItemEquip += ItemSystem.OnEquipLaceration;
      player.oid.LoginCreature.OnItemUnequip += ItemSystem.OnUnEquipLaceration;

      var weapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.RightHand);
      var secondWeapon = player.oid.LoginCreature.GetItemInSlot(InventorySlot.LeftHand);

      if ((weapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.TwoBladedSword, BaseItemType.Bastardsword, BaseItemType.Katana, BaseItemType.Longsword, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle))
        || (secondWeapon is not null && Utils.In(weapon.BaseItem.ItemType, BaseItemType.Scimitar, BaseItemType.Kama, BaseItemType.Kukri, BaseItemType.Sickle)))
      {
        if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.CooldownEffectTag && e.IntParams[5] == CustomSkill.ExpertiseLaceration))
          player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.ExpertiseLaceration, 100);
      }

      return true;
    }
  }
}