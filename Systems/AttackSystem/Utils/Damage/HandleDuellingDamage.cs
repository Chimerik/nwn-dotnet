using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class DamageUtils
  {
    public static void HandleDuellingDamage(OnCreatureDamage onDamage, NwCreature damager)
    {
      if (onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon) < 0 || !PlayerSystem.Players.TryGetValue(damager, out PlayerSystem.Player player)
        || !player.learnableSkills.TryGetValue(CustomSkill.FighterCombatStyleDuel, out LearnableSkill duel) || duel.currentLevel < 1)
        return;

      NwItem weapon = damager.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || ItemUtils.IsTwoHandedWeapon(weapon.BaseItem, damager.Size))
        return;

      NwItem offHandWeapon = damager.GetItemInSlot(InventorySlot.LeftHand);

      if ((ItemUtils.IsVersatileWeapon(weapon.BaseItem.ItemType) && offHandWeapon is null) || (offHandWeapon is not null && ItemUtils.IsWeapon(offHandWeapon.BaseItem)))
        return;

      LogUtils.LogMessage($"Style de combat duel, +2 dégâts appliqués", LogUtils.LogType.Combat);
      onDamage.DamageData.SetDamageByType(DamageType.BaseWeapon, onDamage.DamageData.GetDamageByType(DamageType.BaseWeapon) + 2);
    }
  }
}
